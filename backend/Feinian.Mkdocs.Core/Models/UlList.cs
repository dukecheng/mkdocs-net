namespace Niusys.Docs.Core.Models
{
    public class UlList
    {
        public List<LiItem> Items { get; set; }
        public UlList()
        {
            Items = new List<LiItem>();
        }


        private UlList _level1 => this;
        private UlList _level2;
        private UlList _level3;
        private UlList _level4;

        private UlList _latest;
        //private UlList _level5;
        //private UlList _level6;
        //private UlList _level7;
        //private UlList _level8;
        //private UlList _level9;


        public void AddLevelItem(LiItem item, int level)
        {
            if (item.UlList != null)
            {
                _latest = item.UlList;
            }
            switch (level)
            {
                case 1:
                    _level1.Items.Add(item);
                    _level2 = item.UlList;
                    _level3 = null;
                    _level4 = null;
                    break;
                case 2:
                    _level2.Items.Add(item);
                    _level3 = item.UlList;
                    _level4 = null;
                    break;
                case 3:
                    _level3.Items.Add(item);
                    _level4 = item.UlList;
                    break;
                case 4:
                    _level4.Items.Add(item);
                    break;
                default:
                    throw new Exception($"Not Supported Level {level}");
            }
        }

        public void AddItem(LiItem item)
        {
            if (_latest == null)
            {
                _level1.Items.Add(item);
            }
            else
            {
                _latest.Items.Add(item);
            }
        }
    }
}
