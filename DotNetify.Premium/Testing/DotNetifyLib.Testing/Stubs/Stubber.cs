using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace DotNetify.Testing
{
   public interface IStub<TInterface>
   {
      TInterface Object { get; }

      IStubMethodSetup<TInterface, T> Setup<T>(Expression<Func<TInterface, T>> method);

      IStubMethodSetup<TInterface> Setup(Expression<Action<TInterface>> method);

      void Raise(string eventName, object sender);

      void Raise<T>(string eventName, object sender, T args);
   }

   public interface IStubMethodSetup<TInterface, T>
   {
      IStub<TInterface> Returns(T returns);

      IStub<TInterface> Returns(Func<T> returns);

      IStub<TInterface> Returns<T1>(Func<T1, T> returns);

      IStub<TInterface> Returns<T1, T2>(Func<T1, T2, T> returns);

      IStub<TInterface> Returns<T1, T2, T3>(Func<T1, T2, T3, T> returns);

      IStub<TInterface> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, T> returns);

      IStub<TInterface> Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, T> returns);

      IStub<TInterface> Throws(Exception exception);

      IStub<TInterface> Raises(string eventName, object sender);

      IStub<TInterface> Raises<T1>(string eventName, object sender, T1 arg);

      IStub<TInterface> Callback(Action<TInterface> callback);
   }

   public interface IStubMethodSetup<TInterface>
   {
      IStub<TInterface> Throws(Exception exception);

      IStub<TInterface> Raises(string eventName, object sender);

      IStub<TInterface> Raises<T1>(string eventName, object sender, T1 arg);

      IStub<TInterface> Callback(Action<TInterface> callback);

      IStub<TInterface> Calls<T1>(Action<T1> calls);

      IStub<TInterface> Calls<T1, T2>(Action<T1, T2> calls);

      IStub<TInterface> Calls<T1, T2, T3>(Action<T1, T2, T3> calls);

      IStub<TInterface> Calls<T1, T2, T3, T4>(Action<T1, T2, T3, T4> calls);

      IStub<TInterface> Calls<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> calls);
   }

   public interface IArgumentPattern
   {
      bool IsMatch(object value);
   }

   public static class It
   {
      public class AnyValue<T> : IArgumentPattern
      {
         public bool IsMatch(object value) => value == null
                 || typeof(T).IsAssignableFrom(value.GetType())
                 || Nullable.GetUnderlyingType(typeof(T))?.IsAssignableFrom(value.GetType()) == true;
      }

      public class ByRefValue : AnyValue<object>
      {
         public object Value { get; }

         public ByRefValue(object value)
         {
            Value = value;
         }
      }

      public static T IsAny<T>() => default(T);
   }

   public abstract partial class BaseStub<TInterface>
   {
      internal class MethodSetup<T> : IStubMethodSetup<TInterface, T>, IStubMethodSetup<TInterface>
      {
         private IStub<TInterface> _baseStub;
         private Action<Delegate> _setAction;

         public MethodSetup(IStub<TInterface> baseStub, Action<Delegate> setAction)
         {
            _baseStub = baseStub;
            _setAction = setAction;
         }

         public IStub<TInterface> Returns(T returns) => SetAction(new Func<T>(() => returns));

         public IStub<TInterface> Returns(Func<T> returns) => SetAction(returns);

         public IStub<TInterface> Returns<T1>(Func<T1, T> returns) => SetAction(returns);

         public IStub<TInterface> Returns<T1, T2>(Func<T1, T2, T> returns) => SetAction(returns);

         public IStub<TInterface> Returns<T1, T2, T3>(Func<T1, T2, T3, T> returns) => SetAction(returns);

         public IStub<TInterface> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, T> returns) => SetAction(returns);

         public IStub<TInterface> Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, T> returns) => SetAction(returns);

         public IStub<TInterface> Throws(Exception exception) => SetAction(new Action(() => { throw exception; }));

         public IStub<TInterface> Raises(string eventName, object sender) => SetAction(new Action(() => _baseStub.Raise(eventName, sender)));

         public IStub<TInterface> Raises<T1>(string eventName, object sender, T1 arg) => SetAction(new Action(() => _baseStub.Raise(eventName, sender, arg)));

         public IStub<TInterface> Callback(Action<TInterface> callback) => SetAction(new Action(() => callback(_baseStub.Object)));

         public IStub<TInterface> Calls<T1>(Action<T1> calls) => SetAction(calls);

         public IStub<TInterface> Calls<T1, T2>(Action<T1, T2> calls) => SetAction(calls);

         public IStub<TInterface> Calls<T1, T2, T3>(Action<T1, T2, T3> calls) => SetAction(calls);

         public IStub<TInterface> Calls<T1, T2, T3, T4>(Action<T1, T2, T3, T4> calls) => SetAction(calls);

         public IStub<TInterface> Calls<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> calls) => SetAction(calls);

         private IStub<TInterface> SetAction(Delegate returns)
         {
            _setAction(returns);
            return _baseStub;
         }
      }
   }

   public abstract partial class BaseStub<TInterface> : IStub<TInterface> where TInterface : class
   {
      private Dictionary<string, object> _propValues = new Dictionary<string, object>();
      private List<Tuple<string, object[], Delegate>> _methodReturns = new List<Tuple<string, object[], Delegate>>();
      private Dictionary<string, Delegate> _defaultMethodReturns = new Dictionary<string, Delegate>();
      private List<Tuple<string, Delegate>> _eventHandlers = new List<Tuple<string, Delegate>>();
      private readonly Random _random = new Random();

      public TInterface Object { get; set; }

      public T Get<T>(string propName)
      {
         if (!_propValues.ContainsKey(propName))
            _propValues[propName] = Default<T>();

         return _propValues[propName] is Delegate ? (T) (_propValues[propName] as Delegate).DynamicInvoke() : (T) _propValues[propName];
      }

      public void Set<T>(string propName, T value) => _propValues[propName] = value;

      public void Raise(string eventName, object sender) => Raise(eventName, sender, (object) null);

      public void Raise<T>(string eventName, object sender, T args)
      {
         _eventHandlers
             .Where(tuple => tuple.Item1 == eventName)
             .ToList()
             .ForEach(tuple => tuple.Item2.DynamicInvoke(new object[] { sender, args }));
      }

      public void VoidMethod(string methodName, List<object> args, List<object> setupArgs)
      {
         var match = _methodReturns.FirstOrDefault(tuple => tuple.Item1 == methodName);
         if (match != null)
            Invoke<object>(match.Item3, args.ToArray());
      }

      public T Method<T>(string methodName, List<object> args, List<object> setupArgs)
      {
         var matches = _methodReturns.Where(tuple => tuple.Item1 == methodName && ArgumentsMatch(args, tuple.Item2));
         foreach (var match in matches)
         {
            try
            {
               setupArgs.AddRange(match.Item2.Select(arg => arg is It.ByRefValue ? (arg as It.ByRefValue).Value : arg));
               var matchParamsLength = match.Item3.GetMethodInfo().GetParameters().Length;
               return Invoke<T>(match.Item3, matchParamsLength == args.Count ? args.ToArray() : null);
            }
            catch (InvalidCastException)
            {
               /* No guarantee that there's only one match, so rely on Invoke not throwing invalid cast exception to pick the right one */
            }
         }

         if (!_defaultMethodReturns.ContainsKey(methodName))
         {
            T value = Default<T>();
            _defaultMethodReturns[methodName] = new Func<T>(() => value);
         }

         var returns = _defaultMethodReturns[methodName];
         var paramsLength = returns.GetMethodInfo().GetParameters().Length;
         return Invoke<T>(returns, paramsLength == args.Count ? args.ToArray() : null);
      }

      private T Invoke<T>(Delegate functionDelegate, object[] args)
      {
         try
         {
            return (T) functionDelegate.DynamicInvoke(args);
         }
         catch (InvalidCastException)
         {
            throw;
         }
         catch (Exception ex)
         {
            throw ex.InnerException;
         }
      }

      public bool AddEvent(string name, object value)
      {
         _eventHandlers.Add(Tuple.Create(name, (Delegate) value));
         return true;
      }

      public bool RemoveEvent(string name, object value)
      {
         var idx = _eventHandlers.FindIndex(tuple => tuple.Item1 == name && tuple.Item2 == (Delegate) value);
         if (idx >= 0)
            _eventHandlers.RemoveAt(idx);
         return true;
      }

      public IStubMethodSetup<TInterface> Setup(Expression<Action<TInterface>> method)
      {
         var expression = (MethodCallExpression) method.Body;
         var methodName = expression.Method.ToString();
         return new MethodSetup<object>(this, returns => _methodReturns.Add(new Tuple<string, object[], Delegate>(methodName, null, returns)));
      }

      public IStubMethodSetup<TInterface, T> Setup<T>(Expression<Func<TInterface, T>> method)
      {
         if (method.Body is MemberExpression)
         {
            var propertyExpression = (MemberExpression) method.Body;
            var propertyName = propertyExpression.Member.Name;
            return new MethodSetup<T>(this, returns => _propValues[propertyName] = returns);
         }

         var expression = (MethodCallExpression) method.Body;
         var methodName = expression.Method.ToString();
         var methodParams = expression.Method.GetParameters();

         if (expression.Method.IsGenericMethod)
            methodName = expression.Method.GetGenericMethodDefinition().ToString();

         var methodArgs = expression.Arguments.Select((arg, idx) =>
         {
            object value = null;
            if (arg is MethodCallExpression && (arg as MethodCallExpression).Method.Name == "IsAny")
            {
               Type isAnyType = typeof(It.AnyValue<>).MakeGenericType((arg as MethodCallExpression).Method.ReturnType);
               value = Activator.CreateInstance(isAnyType);
            }
            else if (methodParams[idx].IsOut || methodParams[idx].ParameterType.IsByRef)
               value = Activator.CreateInstance(typeof(It.ByRefValue), Expression.Lambda(arg).Compile().DynamicInvoke());

            return value ?? Expression.Lambda(arg).Compile().DynamicInvoke();
         });

         return new MethodSetup<T>(this, returns => _methodReturns.Add(new Tuple<string, object[], Delegate>(methodName, methodArgs.ToArray(), returns)));
      }

      private bool ArgumentsMatch(List<object> args, object[] argPatterns)
      {
         int length = Math.Min(args.Count, argPatterns.Length);
         bool match = true;
         for (int i = 0; i < length && match; i++)
         {
            if (argPatterns[i] is IArgumentPattern)
               match = (argPatterns[i] as IArgumentPattern).IsMatch(args[i]);
            else if (argPatterns[i] == null)
               match = args[i] == null;
            else
               match = argPatterns[i].Equals(args[i]);
         }
         return match;
      }

      private T Default<T>()
      {
         Type t = typeof(T);
         if (t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong))
            return (T) Convert.ChangeType(_random.Next(1, int.MaxValue), t);
         else if (t == typeof(double) || t == typeof(decimal))
            return (T) Convert.ChangeType(_random.NextDouble() * 100, t);
         else if (t == typeof(byte))
            return (T) Convert.ChangeType(_random.Next(1, 255), t);
         else if (t == typeof(DateTime))
            return (T) Convert.ChangeType(DateTime.Now, t);
         else if (t == typeof(DateTimeOffset))
            return (T) Convert.ChangeType(DateTimeOffset.Now, t);
         else if (t == typeof(string))
         {
            var buffer = new byte[10];
            return (T) Convert.ChangeType(Encoding.UTF8.GetString(buffer.Select(b => (byte) _random.Next('A', 'z')).ToArray()), t);
         }
         else if (t.IsArray)
            return (T) Convert.ChangeType(Array.CreateInstance(t.GetElementType(), _random.Next(2, 20)), t);

         try
         {
            return Activator.CreateInstance<T>();
         }
         catch (Exception)
         { /* swallow */}
         return default(T);
      }
   }

   public static class Stubber
   {
      private static ModuleBuilder _builder;
      private static Dictionary<string, Type> _createdTypes = new Dictionary<string, Type>();
      private static Dictionary<string, bool> _createdEvents = new Dictionary<string, bool>();
      private static readonly object _sync = new object();

      public static IStub<T> Create<T>() where T : class
      {
         lock (_sync)
         {
            Type ifaceType = typeof(T);
            Type baseStubType = typeof(BaseStub<>).MakeGenericType(ifaceType);
            string stubTypeName = $"Stub_{ifaceType.FullName}";

            ModuleBuilder builder = GetModuleBuilder();
            Type stubType = _createdTypes.ContainsKey(stubTypeName) ? _createdTypes[stubTypeName] : null;
            if (stubType == null)
            {
               var typeBuilder = builder.DefineType(stubTypeName, TypeAttributes.Public | TypeAttributes.Class, baseStubType);
               typeBuilder.AddInterfaceImplementation(ifaceType);
               typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

               var typesToBuild = new List<Type>();
               var typesToCheck = new Stack<Type>();
               typesToCheck.Push(ifaceType);
               while (typesToCheck.Count > 0)
               {
                  Type typeToCheck = typesToCheck.Pop();
                  typeToCheck.GetInterfaces().ToList().ForEach(type => typesToCheck.Push(type));
                  typesToBuild.Add(typeToCheck);
               }

               typesToBuild.ForEach(type =>
               {
                  typeBuilder.BuildProperties(type, baseStubType);
                  typeBuilder.BuildEvents(type, baseStubType);
                  typeBuilder.BuildMethods(type, baseStubType);
               });

               stubType = typeBuilder.CreateTypeInfo();
               _createdTypes[stubTypeName] = stubType;
               _createdEvents.Clear();
            }

            object stubInstance = Activator.CreateInstance(stubType);
            var stub = (BaseStub<T>) stubInstance;
            stub.Object = (T) stubInstance;
            return stub;
         }
      }

      private static ModuleBuilder GetModuleBuilder()
      {
         if (_builder != null)
            return _builder;

         var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DotNetifyTestingDynamicAssembly"), AssemblyBuilderAccess.Run);
         _builder = assembly.DefineDynamicModule("MainModule");
         return _builder;
      }

      private static void BuildProperties(this TypeBuilder typeBuilder, Type ifaceType, Type baseStubType)
      {
         foreach (PropertyInfo prop in ifaceType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
         {
            var propBuilder = typeBuilder.DefineProperty(prop.Name, prop.Attributes, prop.PropertyType, null);
            if (prop.CanRead)
            {
               var getMethod = prop.GetGetMethod();
               var methodParams = getMethod.GetParameters();
               bool isIndexer = methodParams.Length > 0;
               var paramTypes = isIndexer ? methodParams.Select(x => x.ParameterType) : Type.EmptyTypes;

               var methodBuilder = typeBuilder.DefineMethod(getMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, prop.PropertyType, paramTypes.ToArray());
               ILGenerator il = methodBuilder.GetILGenerator();
               il.Emit(OpCodes.Ldarg_0);
               if (isIndexer)
               {
                  il.Emit(OpCodes.Ldstr, getMethod.ToString());
                  il.Emit(OpCodes.Ldarg_1);
                  il.Emit(OpCodes.Call, baseStubType.GetMethod("GetIndexer").MakeGenericMethod(prop.PropertyType, paramTypes.First()));
               }
               else
               {
                  il.Emit(OpCodes.Ldstr, prop.Name);
                  il.Emit(OpCodes.Call, baseStubType.GetMethod("Get").MakeGenericMethod(prop.PropertyType));
               }
               il.Emit(OpCodes.Ret);
            }

            if (prop.CanWrite)
            {
               var setMethod = prop.GetSetMethod();

               var baseStubMethod = baseStubType.GetMethod("Set").MakeGenericMethod(setMethod.GetParameters().First().ParameterType);
               var paramTypes = setMethod.GetParameters().Select(paramInfo => paramInfo.ParameterType).ToArray();
               var methodBuilder = typeBuilder.DefineMethod(setMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), paramTypes);
               ILGenerator il = methodBuilder.GetILGenerator();
               il.Emit(OpCodes.Ldarg_0);
               il.Emit(OpCodes.Ldstr, prop.Name);
               il.Emit(OpCodes.Ldarg_1);
               il.Emit(OpCodes.Call, baseStubMethod);
               il.Emit(OpCodes.Ret);
            }
         }
      }

      private static void BuildEvents(this TypeBuilder typeBuilder, Type ifaceType, Type baseStubType)
      {
         foreach (EventInfo eventInfo in ifaceType.GetEvents(BindingFlags.Instance | BindingFlags.Public))
         {
            if (_createdEvents.ContainsKey(eventInfo.Name))
               continue;
            else
               _createdEvents[eventInfo.Name] = true;

            var eventBuilder = typeBuilder.DefineEvent(eventInfo.Name, eventInfo.Attributes, eventInfo.EventHandlerType);
            var eventField = typeBuilder.DefineField(string.Concat("_", eventInfo.Name), eventInfo.EventHandlerType, FieldAttributes.Private);

            //add
            {
               var addMethodInfo = eventInfo.GetAddMethod();
               var methodBuilder = typeBuilder.DefineMethod(addMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { eventInfo.EventHandlerType });
               var ilGenerator = methodBuilder.GetILGenerator();
               var baseStubAddEventMethod = baseStubType.GetMethod("AddEvent");

               ilGenerator.DeclareLocal(typeof(bool), true);
               ilGenerator.Emit(OpCodes.Ldarg_0);
               ilGenerator.Emit(OpCodes.Ldstr, eventInfo.Name);
               ilGenerator.Emit(OpCodes.Ldarg_1);
               ilGenerator.EmitCall(OpCodes.Callvirt, baseStubAddEventMethod, null);
               ilGenerator.Emit(OpCodes.Stloc_0);
               ilGenerator.Emit(OpCodes.Ret);
               typeBuilder.DefineMethodOverride(methodBuilder, addMethodInfo);
            }
            //remove
            {
               var removeMethodInfo = eventInfo.GetRemoveMethod();
               var methodBuilder = typeBuilder.DefineMethod(removeMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { eventInfo.EventHandlerType });
               var ilGenerator = methodBuilder.GetILGenerator();
               var baseStubRemoveEventMethod = baseStubType.GetMethod("RemoveEvent");

               ilGenerator.DeclareLocal(typeof(bool), true);
               ilGenerator.Emit(OpCodes.Ldarg_0);
               ilGenerator.Emit(OpCodes.Ldstr, eventInfo.Name);
               ilGenerator.Emit(OpCodes.Ldarg_1);
               ilGenerator.EmitCall(OpCodes.Callvirt, baseStubRemoveEventMethod, null);
               ilGenerator.Emit(OpCodes.Stloc_0);
               ilGenerator.Emit(OpCodes.Ret);
               typeBuilder.DefineMethodOverride(methodBuilder, removeMethodInfo);
            }
         }
      }

      private static void BuildMethods(this TypeBuilder typeBuilder, Type ifaceType, Type baseStubType)
      {
         foreach (MethodInfo method in ifaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(method => !method.IsSpecialName))
         {
            var baseStubMethod = method.ReturnType == typeof(void)
                ? baseStubType.GetMethod("VoidMethod")
                : baseStubType.GetMethod("Method").MakeGenericMethod(method.ReturnType);
            var paramTypes = method.GetParameters().Select(paramInfo => paramInfo.ParameterType).ToArray();
            var methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.ReturnType, paramTypes);

            var genericArgumentArray = method.GetGenericArguments();
            if (genericArgumentArray.Any())
               methodBuilder.DefineGenericParameters(genericArgumentArray.Select(arg => arg.Name).ToArray());

            ILGenerator il = methodBuilder.GetILGenerator();

            il.BuildArgumentList(method.GetParameters());
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, method.ToString());
            il.Emit(OpCodes.Ldloc_0);

            il.DeclareLocal(typeof(List<object>), true);
            il.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);

            il.Emit(OpCodes.Call, baseStubMethod);
            il.SetByRefArguments(method.GetParameters());
            il.Emit(OpCodes.Ret);
         }
      }

      private static void BuildArgumentList(this ILGenerator il, ParameterInfo[] methodParams)
      {
         var addListMethod = typeof(List<object>).GetMethod("Add", new Type[] { typeof(object) });
         var byRefValueTypes = new Dictionary<Type, Action<Type>>
            {
                {typeof(bool),      t => il.Emit(OpCodes.Ldind_U1) },
                {typeof(short),     t => il.Emit(OpCodes.Ldind_I2) },
                {typeof(int),       t => il.Emit(OpCodes.Ldind_I4) },
                {typeof(float),     t => il.Emit(OpCodes.Ldind_R4) },
                {typeof(double),    t => il.Emit(OpCodes.Ldind_R8) },
                {typeof(decimal),   t => il.Emit(OpCodes.Ldobj, t) }
            };

         il.DeclareLocal(typeof(List<object>), true);
         il.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructor(Type.EmptyTypes));
         il.Emit(OpCodes.Stloc_0);

         for (int i = 0; i < methodParams.Length; i++)
         {
            var methodParam = methodParams[i];

            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldarg, i + 1);

            if (methodParam.ParameterType.IsByRef || methodParam.IsOut)
            {
               var type = methodParam.ParameterType.GetElementType();
               if (byRefValueTypes.ContainsKey(type))
               {
                  byRefValueTypes[type](type);
                  il.Emit(OpCodes.Box, type);
               }
               else
                  il.Emit(OpCodes.Ldind_Ref);
            }
            else if (methodParam.ParameterType.IsValueType || methodParam.ParameterType.IsGenericParameter)
               il.Emit(OpCodes.Box, methodParams[i].ParameterType);

            il.EmitCall(OpCodes.Callvirt, addListMethod, null);
         }
      }

      private static void SetByRefArguments(this ILGenerator il, ParameterInfo[] methodParams)
      {
         var getListMethod = typeof(List<object>).GetMethod("get_Item");
         var byRefValueTypes = new Dictionary<Type, Action<Type>>
            {
                {typeof(bool),      t => il.Emit(OpCodes.Stind_I1) },
                {typeof(short),     t => il.Emit(OpCodes.Stind_I2) },
                {typeof(int),       t => il.Emit(OpCodes.Stind_I4) },
                {typeof(float),     t => il.Emit(OpCodes.Stind_R4) },
                {typeof(double),    t => il.Emit(OpCodes.Stind_R8) },
                {typeof(string),    t => il.Emit(OpCodes.Stind_Ref) }
            };

         for (int i = 0; i < methodParams.Length; i++)
         {
            var methodParam = methodParams[i];
            if (methodParam.ParameterType.IsByRef || methodParam.IsOut)
            {
               var type = methodParam.ParameterType.GetElementType();
               if (byRefValueTypes.ContainsKey(type))
               {
                  il.Emit(OpCodes.Ldarg, i + 1);
                  il.Emit(OpCodes.Ldloc_1);
                  il.Emit(OpCodes.Ldc_I4, i);
                  il.EmitCall(OpCodes.Callvirt, getListMethod, null);
                  il.Emit(OpCodes.Unbox_Any, type);
                  byRefValueTypes[type](type);
               }
            }
         }
      }
   }
}