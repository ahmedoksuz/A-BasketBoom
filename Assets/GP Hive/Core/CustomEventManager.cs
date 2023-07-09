using System;
using System.Collections.Generic;

namespace GPHive.Core
{
    public static class CustomEventManager
    {
        private static Dictionary<string, Action> eventDictionary = new Dictionary<string, Action>();

        public static void Subscribe(string eventName, Action listener)
        {
            if (!eventDictionary.ContainsKey(eventName))
                eventDictionary.Add(eventName, listener);
            else
                eventDictionary[eventName] += listener;
        }

        public static void Unsubscribe(string eventName, Action listener)
        {
            if (eventDictionary.ContainsKey(eventName))
                eventDictionary[eventName] -= listener;
        }

        public static void TriggerEvent(string eventName)
        {
            if (eventDictionary.ContainsKey(eventName))
                eventDictionary[eventName]?.Invoke();
        }
    }
}
