namespace Assets.Soraphis.SaveGame {
    public interface Saveable {
        void Load(DataNode parent);
        DataNode Save();

        void CreateDefault();
    }
}
