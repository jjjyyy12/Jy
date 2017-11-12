using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Jy.IIndex
{
    /// <summary>
    /// 创建Index实例的工具类，emit方式
    /// </summary>
    public class CreateIndex
    {
        /// <summary>
        /// 配置实例
        /// </summary>
        private Dictionary<Type, Type> _configDictionary = new Dictionary<Type, Type>();
        /// <summary>
        /// 添加配置
        /// </summary>
        /// <typeparam name="TInterface">接口</typeparam>
        /// <typeparam name="TType">实现接口的类型</typeparam>
        public void AddConfig<TInterface, TType>()
        {
            //判断TType是否实现TInterface
            if (typeof(TInterface).IsAssignableFrom(typeof(TType)))
            {
                _configDictionary.Add(typeof(TInterface), typeof(TType));
            }
            else
            {
                throw new Exception($"类型未实现接口:{typeof(TInterface).Name}");
            }
        }

        public Dictionary<Type, Type> ConfigDictionary
        {
            get
            {
                return _configDictionary;
            }
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <typeparam name="TInterface">接口</typeparam>
        /// <returns></returns>
        public TInterface Get<TInterface>(object[] parameters)
        {
            Type type;
            var can = _configDictionary.TryGetValue(typeof(TInterface), out type);
            if (can)
            {
                //BindingFlags defaultFlags = BindingFlags.Public | BindingFlags.Instance;
                //var constructors = type.GetConstructors(defaultFlags);//获取默认构造函数
                var constructor = type.GetConstructor(new Type[1] { parameters[0].GetType() });//JyDBReadContext //带一个参数的构造方法
                //return (TInterface)constructor.Invoke(parameters);
                var t = (TInterface)this.CreateInstanceByEmit(constructor, parameters);
                return t;
            }
            else
            {
                throw new Exception($"未找到对应的类型:{typeof(TInterface).Name}");
            }
        }
        /// <summary>
        /// 实例化对象 用EMIT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructor"></param>
        /// <returns></returns>
        private Object CreateInstanceByEmit(ConstructorInfo constructor, object[] parameters)
        {
            //动态方法
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString("N"), typeof(Object), new[] { parameters[0].GetType() });
            //方法IL
            ILGenerator il = dynamicMethod.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); //put JyDBReadContext into stack
            //实例化命令
            il.Emit(OpCodes.Newobj, constructor);
            //返回
            il.Emit(OpCodes.Ret);

            ///执行方法
            return dynamicMethod.Invoke(new object(), new object[] { parameters[0] });
            ///delegate方式
            //var func = (Func<JyDBReadContext, Object>)dynamicMethod.CreateDelegate(typeof(Func<JyDBReadContext, Object>));
            //return func.Invoke((JyDBReadContext)parameters[0]);
        }
    }
}
