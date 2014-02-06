using System;
using System.Collections.Generic;
namespace MonoPlatformerGame
{
    class TimeList
    {
        //private static List<int> times = new List<int>();
        //private static Dictionary<int, KeyValuePair<string, TimeSpan>> elapsedTimes = new Dictionary<int, KeyValuePair<string, TimeSpan>>();
        private static List<KeyValuePair<string, TimeSpan>> times = new List<KeyValuePair<string, TimeSpan>>();
        //OrderedEnumerable<KeyValuePair<int,KeyValuePair<string,TimeSpan>>,TimeSpan>
            
        public static void AddTime(int id, TimeSpan time)
        {
            string name = "";
            ClientInfo  ci;
            NetManager.connectedClients.TryGetValue(id, out ci);

            if (ci != null)
                name = ci.Name;
            else if (id == 0 && NetManager.IsHost)
                name = DataStorage.GetLocalPlayerInfo().UserName;

            //elapsedTimes.Add(id, new KeyValuePair<string, TimeSpan>(name, time));
            //elapsedTimes.OrderBy(pair => pair.Value.Value);
            times.Add(new KeyValuePair<string, TimeSpan>(name, time));
            //times.OrderBy(pair => pair.Value);

            foreach (var item in times)
            {
                Log.WriteLine(item.Key + " - " + item.Value);
            }
            
        }


    }
}
