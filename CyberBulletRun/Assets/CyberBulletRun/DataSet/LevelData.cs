using System;
using System.Collections.Generic;

namespace CyberBulletRun.DataSet {
    [Serializable]
    public class LevelData {
        public int Id;
        public List<string> Prefabs;
        public string StartPlatform;
        public List<int> Enemy;
    }
}
