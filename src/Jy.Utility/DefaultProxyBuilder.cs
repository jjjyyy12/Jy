//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Reflection;
//using System.Reflection.Emit;


//namespace Jy.Utility
//{
//    public static class DefaultProxyBuilder
//    {
//        private static readonly Type VoidType = Type.GetType("System.Void"); //函数返回 void类型

//        public static T CreateProxy<T>()
//        {
//            Type classType = typeof(T);

//            string name = classType.Namespace + ".Aop"; //新的命名空间
//            string fileName = name + ".dll";


//            var assemblyName = new AssemblyName(name);
//            var assemblyBuilder =   AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
//            var moduleBuilder = assemblyBuilder.DefineDynamicModule(name, fileName);//构建命名空间
//            var aopType = BulidType(classType, moduleBuilder);

//            assemblyBuilder.Save(fileName);
//            return (T)Activator.CreateInstance(aopType);
//        }

//        private static Type BulidType(Type classType, ModuleBuilder moduleBuilder)
//        {
//            string className = classType.Name + "_Proxy";

//            //定义类型
//            var typeBuilder = moduleBuilder.DefineType(className,
//                                                       TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
//                                                       classType);//定义一个新类，继承classType
//                                                                  //定义字段 _inspector 声明一个私有且只读的变量
//            var inspectorFieldBuilder = typeBuilder.DefineField("_inspector", typeof(IInterceptor),
//                                                                FieldAttributes.Private | FieldAttributes.InitOnly);
//            //构造函数 
//            BuildCtor(classType, inspectorFieldBuilder, typeBuilder);

//            //构造方法
//            BuildMethod(classType, inspectorFieldBuilder, typeBuilder);
//            Type aopType = typeBuilder.CreateType();
//            return aopType;
//        }

//        private static void BuildMethod(Type classType, FieldBuilder inspectorFieldBuilder, TypeBuilder typeBuilder)
//        {
//            var methodInfos = classType.GetMethods();//获取 原类型
//            foreach (var methodInfo in methodInfos)// 获取 虚函数 或 抽象函数
//            {
//                if (!methodInfo.IsVirtual && !methodInfo.IsAbstract)
//                    continue;
//                if (methodInfo.Name == "ToString")
//                    continue;
//                if (methodInfo.Name == "GetHashCode")
//                    continue;
//                if (methodInfo.Name == "Equals")
//                    continue;

//                var parameterInfos = methodInfo.GetParameters();
//                var parameterTypes = parameterInfos.Select(p => p.ParameterType).ToArray();
//                var parameterLength = parameterTypes.Length;
//                var hasResult = methodInfo.ReturnType != VoidType;

//                var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name,
//                                                             MethodAttributes.Public | MethodAttributes.Final |
//                                                             MethodAttributes.Virtual
//                                                             , methodInfo.ReturnType
//                                                             , parameterTypes);

//                var il = methodBuilder.GetILGenerator();

//                //局部变量 声明
//                il.DeclareLocal(typeof(object)); //correlationState (emit 标签从0 开始:loc_0,loc_1,loc_2)
//                il.DeclareLocal(typeof(object)); //result
//                il.DeclareLocal(typeof(object[])); //parameters

//                #region BeforeCall
//                //BeforeCall(string operationName, object[] inputs);
//                il.Emit(OpCodes.Ldarg_0);

//                il.Emit(OpCodes.Ldfld, inspectorFieldBuilder);//获取字段_inspector
//                il.Emit(OpCodes.Ldstr, methodInfo.Name);//参数operationName

//                if (parameterLength == 0)//判断方法参数长度
//                {
//                    il.Emit(OpCodes.Ldnull);//null -> 参数 inputs
//                }
//                else
//                {
//                    //创建new object[parameterLength];
//                    il.Emit(OpCodes.Ldc_I4, parameterLength);
//                    il.Emit(OpCodes.Newarr, typeof(Object));
//                    il.Emit(OpCodes.Stloc_2);//压入局部变量2 parameters

//                    for (int i = 0, j = 1; i < parameterLength; i++, j++)
//                    {
//                        //object[i] = arg[j]
//                        il.Emit(OpCodes.Ldloc_2);
//                        il.Emit(OpCodes.Ldc_I4, 0);
//                        il.Emit(OpCodes.Ldarg, j);
//                        if (parameterTypes[i].IsValueType)
//                            il.Emit(OpCodes.Box, parameterTypes[i]);//对值类型装箱
//                        il.Emit(OpCodes.Stelem_Ref);
//                    }
//                    il.Emit(OpCodes.Ldloc_2);//取出局部变量2 parameters-> 参数 inputs
//                }

//                il.Emit(OpCodes.Callvirt, typeof(IInterceptor).GetMethod("BeforeCall"));//调用BeforeCall
//                il.Emit(OpCodes.Stloc_0);//建返回压入局部变量0 correlationState
//                #endregion

//                #region base.Call
//                //Call methodInfo
//                il.Emit(OpCodes.Ldarg_0); // arg_0: 
//                                          //获取参数表
//                for (int i = 1, length = parameterLength + 1; i < length; i++)
//                {
//                    il.Emit(OpCodes.Ldarg_S, i);
//                }
//                il.Emit(OpCodes.Call, methodInfo);
//                //将返回值压入 局部变量1result void就压入null
//                if (!hasResult)
//                    il.Emit(OpCodes.Ldnull);
//                else if (methodInfo.ReturnType.IsValueType)
//                    il.Emit(OpCodes.Box, methodInfo.ReturnType);//对值类型装箱
//                il.Emit(OpCodes.Stloc_1); //返回值 保存到 result
//                #endregion

//                #region AfterCall
//                //AfterCall(string operationName, object returnValue, object correlationState);
//                il.Emit(OpCodes.Ldarg_0);
//                il.Emit(OpCodes.Ldfld, inspectorFieldBuilder);//获取字段_inspector
//                il.Emit(OpCodes.Ldstr, methodInfo.Name);//参数 operationName
//                il.Emit(OpCodes.Ldloc_1);//局部变量1 result
//                il.Emit(OpCodes.Ldloc_0);// 局部变量0 correlationState
//                il.Emit(OpCodes.Callvirt, typeof(IInterceptor).GetMethod("AfterCall"));
//                #endregion

//                #region result
//                //result 返回结果: void类型 直接返回函数,否则...
//                if (!hasResult)
//                {
//                    il.Emit(OpCodes.Ret);
//                    return;
//                }
//                il.Emit(OpCodes.Ldloc_1);//非void取出局部变量1 result
//                if (methodInfo.ReturnType.IsValueType)
//                    il.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);//对值类型拆箱
//                il.Emit(OpCodes.Ret);
//                #endregion
//            }
//        }

//        private static void BuildCtor(Type classType, FieldBuilder inspectorFieldBuilder, TypeBuilder typeBuilder)
//        {
//            {  //定义 代理类的 构造器
//                var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis,
//                                                                Type.EmptyTypes);
//                var il = ctorBuilder.GetILGenerator();

//                il.Emit(OpCodes.Ldarg_0);
//                il.Emit(OpCodes.Call, classType.GetConstructor(Type.EmptyTypes));//调用base的默认ctor
//                il.Emit(OpCodes.Ldarg_0);
//                //将typeof(classType)压入计算堆
//                il.Emit(OpCodes.Ldtoken, classType);
//                il.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", new[] { typeof(RuntimeTypeHandle) }));
//                //调用DefaultInterceptorFactory.Create(type)
//                il.Emit(OpCodes.Call, typeof(DefaultInterceptorFactory).GetMethod("Create", new[] { typeof(Type) }));
//                //将结果保存到字段_inspector
//                il.Emit(OpCodes.Stfld, inspectorFieldBuilder);
//                il.Emit(OpCodes.Ret);
//            }
//        }


//static Func<object> BuildMethodMyCreateInstance(Type type)
//{
//    DynamicMethod dm = new DynamicMethod(string.Empty, typeof(object), Type.EmptyTypes);
//    var gen = dm.GetILGenerator();
//    if (type.IsValueType)
//    {
//        gen.DeclareLocal(type);
//        gen.Emit(OpCodes.Ldloca_S, 0);
//        gen.Emit(OpCodes.Initobj, type);
//        gen.Emit(OpCodes.Ldloc_0);
//        gen.Emit(OpCodes.Box, type);
//    }
//    else
//    {
//        gen.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
//    }
//    gen.Emit(OpCodes.Ret);
//    return (Func<object>)dm.CreateDelegate(typeof(Func<object>));
//}
//    }
//}
