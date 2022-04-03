namespace lvl0
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    public static class EventBus
    {
        private static Dictionary<Type, ClassMap> m_classRegisterMap = new Dictionary<Type, ClassMap>();
        private static Dictionary<Type, Action<IEvent>> m_cachedRaise = new Dictionary<Type, Action<IEvent>>();

        private class BusMap
        {
            public Action<IEventReceiverBase> Register;
            public Action<IEventReceiverBase> Unregister;
        }

        private class ClassMap
        {
            public BusMap[] buses;
        }

        public static void Initalize() { }

        static EventBus()
        {
            Dictionary<Type, BusMap> busRegisterMap = new Dictionary<Type, BusMap>();

            Type delegateType = typeof(Action<>);
            Type delegateGenericRegister = delegateType.MakeGenericType(typeof(IEventReceiverBase));
            Type delegateGenericRaise = delegateType.MakeGenericType(typeof(IEvent));

            var types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (var t in types)
            {
                if (typeof(IEvent).IsAssignableFrom(t) && t != typeof(IEvent))
                {
                    Type eventHubType = typeof(EventBus<>);
                    Type genMyClass = eventHubType.MakeGenericType(t);
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(genMyClass.TypeHandle);

                    BusMap busMap = new BusMap()
                    {
                        Register = Delegate.CreateDelegate(delegateGenericRegister, genMyClass.GetMethod("Register")) as Action<IEventReceiverBase>,
                        Unregister = Delegate.CreateDelegate(delegateGenericRegister, genMyClass.GetMethod("UnRegister")) as Action<IEventReceiverBase>
                    };

                    busRegisterMap.Add(t, busMap);
                }
            }
            
            foreach (var t in types)
            {
                if (typeof(IEventReceiverBase).IsAssignableFrom(t) && !t.IsInterface)
                {
                    Type[] interfaces = t.GetInterfaces().Where(x => x != typeof(IEventReceiverBase) && typeof(IEventReceiverBase).IsAssignableFrom(x)).ToArray();

                    ClassMap map = new ClassMap()
                    {
                        buses = new BusMap[interfaces.Length]
                    };

                    for (int i = 0; i < interfaces.Length; i++)
                    {
                        var arg = interfaces[i].GetGenericArguments()[0];
                        map.buses[i] = busRegisterMap[arg];
                    }

                    m_classRegisterMap.Add(t, map);
                }
            }
        }

        public static void Register(IEventReceiverBase target)
        {
            Type t = target.GetType();
            ClassMap map = m_classRegisterMap[t];

            foreach (var busMap in map.buses)
            {
                busMap.Register(target);
            }
        }

        public static void UnRegister(IEventReceiverBase target)
        {
            Type t = target.GetType();
            ClassMap map = m_classRegisterMap[t];

            foreach (var busMap in map.buses)
            {
                busMap.Unregister(target);
            }
        }

        public static void Raise(IEvent ev)
        {
            m_cachedRaise[ev.GetType()](ev);
        }
    }
}
