using System;
using System.Reflection;
using Jy.DistributedLock;
using System.Threading;
using System.Reflection.Emit;
using Jy.IMessageQueue;

namespace Jy.DistributedLockHandler
{
    public class ProcessMessageLockHandler
    {
        private readonly Redlock _redlock; //分布式锁
        public ProcessMessageLockHandler(Redlock redlock)
        {
            _redlock = redlock;
        }
        /// <summary>
        /// 动态执行lock方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sender">源实例对象</param>
        /// <param name="type">实例类型</param>
        /// <param name="paras">实例方法的参数</param>
        public void RunLock0<T>( object sender, Type type ,object[] paras) where T : Attribute
        {
            var members = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var member in members)
            {
                if (member.Name.StartsWith("get_") || member.Name.StartsWith("set_") || member.Name.StartsWith("add_") || member.Name.StartsWith("remove_")) continue;
                {
                    Lock lockobj = null;
                    int i = 0;
                    do
                    {
                        if (LockMethod<T>(member, out lockobj))
                        {
                            if (lockobj != null)
                            {
                                member.Invoke(sender, paras);
                                UnLockMethod<T>(member, lockobj);
                            }
                            else member.Invoke(sender, paras);
                            break;
                        }
                        else//锁定失败后继续等待后重新申请锁定，failtimes*2秒
                        {
                            i++;
                            Thread.Sleep(2000*i);
                            if (LockMethod<T>(member, out lockobj))
                            {
                                member.Invoke(sender, paras);
                                UnLockMethod<T>(member, lockobj);
                                break;
                            }
                            else continue;
                        }
                    } while (i <= 4);//4次机会

                }
            }//------end foreach
        }

        public void RunLock<T, TMessageBase>(object sender, Type type, object[] paras) where T : Attribute where TMessageBase:MessageBase
        {
            //动态方法
            var dynamicMethod = new DynamicMethod(type.Name+"_p", null, new Type[] {type, typeof(TMessageBase) });
            //方法IL
            ILGenerator il = dynamicMethod.GetILGenerator();
           
            var members = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var member in members)
            {
                if (member.Name.ToUpper()!= "PROCESSMSG") continue; //拦截ProcessMsg方法，上redis锁，之后动态emit调用后面的实现
                {
                    il.Emit(OpCodes.Ldarg_0); //put IProcessMessage into stack
                    il.Emit(OpCodes.Ldarg_1); //put MessageBase into stack
                    if (type == member.DeclaringType) //call IProcessMessage.ProcessMsg
                        il.Emit(OpCodes.Call, member);
                    else
                        il.Emit(OpCodes.Callvirt, member);
                    il.Emit(OpCodes.Ret);

                    Lock lockobj = null;
                    int i = 0;
                    do
                    {
                        if (LockMethod<T>(member, out lockobj))
                        {
                            if (lockobj != null)
                            {
                                dynamicMethod.Invoke(new object(), new object[] { sender, paras[0] });//动态调用sender中的PROCESSMSG，参数是paras[0]
                                // member.Invoke(sender, paras);
                                UnLockMethod<T>(member, lockobj);
                            }
                            else
                                dynamicMethod.Invoke(new object(), new object[] { sender, paras[0] });
                            //member.Invoke(sender, paras);
                            break;
                        }
                        else//锁定失败后继续等待后重新申请锁定，failtimes*2秒
                        {
                            i++;
                            Thread.Sleep(2000 * i);
                            if (LockMethod<T>(member, out lockobj))
                            {
                                dynamicMethod.Invoke(new object(), new object[] { sender, paras[0] });
                                //member.Invoke(sender, paras);
                                UnLockMethod<T>(member, lockobj);
                                break;
                            }
                            else continue;
                        }
                    } while (i <= 4);//4次机会

                }
            }//------end foreach
        }

        /// <summary>
        /// 锁定方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">方法信息</param>
        /// <param name="lockobj">redlock所需的lockobj</param>
        /// <returns>false表示锁定失败</returns>
        private bool LockMethod<T>(MethodInfo method, out Lock lockobj) where T : Attribute
        {
            lockobj = null;
            Attribute attrib = method.GetCustomAttribute(typeof(T));
            if (attrib != null)
            {
                var keyname = attrib.GetType().GetProperty("keyname").GetValue(attrib).ToString();
                var exptime = (int)attrib.GetType().GetProperty("exptime").GetValue(attrib);
                return _redlock != null ? _redlock.Lock(keyname, new TimeSpan(0,0,exptime), out lockobj) : false;
            }
            return true;
        }
        private void UnLockMethod<T>(MethodInfo method, Lock lockobj) where T : Attribute
        {
            Attribute attrib = method.GetCustomAttribute(typeof(T));
            if (attrib == null || lockobj == null || _redlock == null) return;
            _redlock.Unlock(lockobj);
        }
    }

}
