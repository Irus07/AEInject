using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace DIFactoryGenerator.Accessors
{
    internal static class PrivateFieldAccessor<T>
    {
        private static readonly ConcurrentDictionary<string, Delegate> _settersCache =
            new ConcurrentDictionary<string, Delegate>();

        public static void SetField<TField>(T instance, string fieldName, TField value)
        {
            var cacheKey = fieldName;

            if (!_settersCache.TryGetValue(cacheKey, out var setterDelegate))
            {
                setterDelegate = CreateSetter<TField>(fieldName);
                _settersCache[cacheKey] = setterDelegate;
            }

            var setter = (Action<T, TField>)setterDelegate;
            setter(instance, value);
        }

        private static Action<T, TField> CreateSetter<TField>(string fieldName)
        {
            var fieldInfo = typeof(T).GetField(fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (fieldInfo == null)
                throw new ArgumentException($"Field {fieldName} not found");

            if (fieldInfo.FieldType != typeof(TField))
                throw new ArgumentException($"Field type mismatch");

            var method = new DynamicMethod(
                name: $"Set_{fieldName}",
                returnType: null,
                parameterTypes: new[] { typeof(T), typeof(TField) },
                owner: typeof(T),
                skipVisibility: true
            );

            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldInfo);
            il.Emit(OpCodes.Ret);

            return (Action<T, TField>)method.CreateDelegate(typeof(Action<T, TField>));
        }


    }
}
