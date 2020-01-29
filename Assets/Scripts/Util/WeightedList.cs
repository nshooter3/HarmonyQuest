namespace HarmonyQuest.Util
{
    using UnityEngine;
    using System.Collections.Generic;

    public class WeightedList<T>
    {
        private List<WeightedListNode> weightedList = new List<WeightedListNode>();

        /// <summary>
        /// Int storing the highest priority value that has been added to this list.
        /// Used in GetListWithOnlyHighestPriorityNodes to ignore lower priority nodes when making a random selection
        /// </summary>
        private int highestPriority = 1;

        /// <summary>
        /// Add an entry to this weighted list
        /// </summary>
        /// <param name="entry"> The entry to add </param>
        /// <param name="weight"> The entry's weight. The higher it is, the more likely this entry is to be selected </param>
        /// <param name="priority"> The entry's priority. When selecting a random outcome, only the highest priority options will be considered </param>
        public void Add(T entry, int weight = 1, int priority = 1)
        {
            if (weight < 1)
            {
                Debug.LogWarning("WeightedList Warning: Weight of " + weight + " is invalid. Overriding weight to be 1, the minimum acceptable value.");
                weight = 1;
            }

            if (priority < 1)
            {
                Debug.LogWarning("WeightedList Warning: Priority of " + priority + " is invalid. Overriding priority to be 1, the minimum acceptable value.");
                priority = 1;
            }

            weightedList.Add(new WeightedListNode(entry, weight, priority));
            if (priority > highestPriority)
            {
                highestPriority = priority;
            }
        }

        /// <summary>
        /// Remove all entries from this list
        /// </summary>
        public void Clear()
        {
            highestPriority = 1;
            weightedList = new List<WeightedListNode>();
        }

        /// <summary>
        /// Returns an random entry from weightedList, with higher weighted entries having better odds
        /// </summary>
        /// <returns> Our random entry </returns>
        public T GetRandomWeightedEntry()
        {
            if (weightedList.Count == 0)
            {
                Debug.LogWarning("WeightedList Warning: GetRandomWeightedEntry called on an empty weighted list.");
                return default(T);
            }

            //Since outcomes beneath the highest priority are to be ignored, create a list that omits lower priority nodes.
            List<WeightedListNode> weightedListHighestPriority = GetListWithOnlyHighestPriorityNodes();

            int totalWeight = 0;
            foreach (WeightedListNode node in weightedListHighestPriority)
            {
                totalWeight += node.Weight;
            }

            int randomValue = Random.Range(1, totalWeight + 1);
            int resultIndex = 0;
            int total = 0;
            for (resultIndex = 0; resultIndex < weightedListHighestPriority.Count; resultIndex++)
            {
                total += weightedListHighestPriority[resultIndex].Weight;
                if (total >= randomValue) break;
            }

            return weightedListHighestPriority[resultIndex].Obj;
        }

        /// <summary>
        /// Return a version of our weighted list that only includes the highest priority nodes. 
        /// </summary>
        /// <returns> A version of our weighted list that only includes the highest priority nodes. </returns>
        private List<WeightedListNode> GetListWithOnlyHighestPriorityNodes()
        {
            List<WeightedListNode> weightedListHighestPriority = weightedList;

            for (int i = 0; i < weightedListHighestPriority.Count; i++)
            {
                if (weightedListHighestPriority[i].Priority < highestPriority)
                {
                    weightedListHighestPriority.RemoveAt(i);
                    i--;
                }
            }

            return weightedListHighestPriority;
        }

        public override string ToString()
        {
            string str = "Start Print Weighted List\n";
            foreach (WeightedListNode node in weightedList)
            {
                str += "Node of value: " + node.Obj.ToString() + ", weight: " + node.Weight + ", priority: " + node.Priority + "\n";
            }
            str += "Highest priority = " + highestPriority + "\nEnd Print Weighted List";
            return str;
        }

        private class WeightedListNode
        {
            public T Obj { get; private set; }
            public int Weight { get; private set; }
            public int Priority { get; private set; }

            /// <summary>
            /// Constructor for our weighted list node
            /// </summary>
            /// <param name="obj"> The object stored in this entry </param>
            /// <param name="weight"> This entry's weight </param>
            /// <param name="priority"> The entry's priority. When selecting a random outcome, only the highest priority options will be considered </param> </param>
            public WeightedListNode(T obj, int weight, int priority)
            {
                Obj = obj;
                Weight = weight;
                Priority = priority;
            }
        }
    }
}
