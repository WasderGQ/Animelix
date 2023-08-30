namespace ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element.__Common__
{
    public interface IVideoButton 
    {
        public void InIt();
        public bool ButtonStatus { get; }
        public void ChangeToTrueStatus();
        public void ChangeToFalseStatus();
        public void OutIt();
    }
}
