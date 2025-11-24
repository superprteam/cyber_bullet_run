using System;

namespace CyberBulletRun.DataSet {
    [Serializable]
    public struct SkinData {
        public int Id;
        public string Name;

        public override string ToString() {
            return "skin" + Id;
        }
        
    }
}


