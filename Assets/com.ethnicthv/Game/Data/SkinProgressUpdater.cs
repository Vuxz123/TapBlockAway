namespace com.ethnicthv.Game.Data
{
    public static class SkinProgressUpdater
    {
        public static void Update(int skinId, GameInternalSetting.SkinProgressConfig config)
        {
            switch (config.Type)
            {
                case GameInternalSetting.SkinProgressType.Gift:
                    UpdateTypeGift(skinId, config);
                    break;
                case GameInternalSetting.SkinProgressType.PlayProgress:
                    UpdateTypeProgress(skinId, config);
                    break;
                case GameInternalSetting.SkinProgressType.Ads:
                case GameInternalSetting.SkinProgressType.Purchase:
                case GameInternalSetting.SkinProgressType.Gatcha:
                default:
                    break;
            }
        }

        private static void UpdateTypeProgress(int skinId, GameInternalSetting.SkinProgressConfig config)
        {
            float progress;
            var levelUnlock = config.LevelUnlock;
            var numberOfCompleted = SaveManager.instance.gameProgressData.GetNumberOfCompletedLevels();
            if (numberOfCompleted >= levelUnlock)
            {
                progress = 1;
            }
            else
            {
                progress = numberOfCompleted / (float)levelUnlock;
            }

            SaveManager.instance.skinProgressData.SetSkinProgress(skinId, progress);
        }

        private static void UpdateTypeGift(int skinId, GameInternalSetting.SkinProgressConfig config)
        {
            if (config.Condition())
            {
                SaveManager.instance.skinProgressData.UnlockSkin(skinId);
            }
        }
    }
}