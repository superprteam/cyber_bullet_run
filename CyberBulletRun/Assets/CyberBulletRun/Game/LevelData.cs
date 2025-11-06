using System;
using System.Collections.Generic;

namespace CyberBulletRun.Game {
    [Serializable]
    public class LevelData {
        public List<string> Prefabs;
        public string StartPlatform;
        public List<string> Enemy;
        public int Length {
            get { return Enemy.Count; }
        }
    }
}
