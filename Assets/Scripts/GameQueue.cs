using System;
using System.Collections.Generic;
using System.Linq;


namespace Assets.Scripts {

    public static class GameQueueExtensions {
        public static int Count(this GameQueue q, GameQueue.Type t) {
            return q.Get(t).Count;
        }
    }

    public class GameQueue {

        public enum Type {
            WorldMessage, BattleMessage, Menu, Battle, Evolve
        }

        public class Element {
            public Action DoAction;
        }

        private readonly Dictionary<Type, Queue<Element>> queue;

        public GameQueue() {
            queue = new Dictionary<GameQueue.Type, Queue<GameQueue.Element>>();
            foreach(var val in Enum.GetValues(typeof(Type)).Cast<Type>()) {
                queue.Add(val, new Queue<Element>());
            }
        }

        public Queue<Element> Get(Type t) {
            return queue[t];
        }

        public Element Dequeue(Type t) {
            return queue[t].Dequeue();
        }

        public void Enqueue(Type t, Element m) {
            queue[t].Enqueue(m);
        }

        public int Count {
            get {
                int i = 0;
                foreach (var val in Enum.GetValues(typeof(Type)).Cast<Type>()) {
                    i += queue[val].Count;
                }
                return i;
            }
        }

    }
}
