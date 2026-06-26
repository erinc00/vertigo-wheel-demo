using UnityEngine;

namespace Vertigo.Core
{
    public static class WalletRepository
    {
        private const string Key = "vertigo_wallet_save";

        public static void Save(Wallet wallet)
        {
            var data = new WalletSaveData
            {
                gold = wallet.Gold,
                cash = wallet.Cash
            };
            for (int i = 0; i < wallet.PermanentItems.Count; i++)
            {
                var item = wallet.PermanentItems[i];
                data.items.Add(new WalletItemSave { id = item.Id, amount = item.Amount });
            }
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(Key, json);
            PlayerPrefs.Save();
        }

        public static Wallet Load(RewardDatabase database, long startGold, long startCash)
        {
            if (!PlayerPrefs.HasKey(Key))
                return new Wallet(startGold, startCash);

            string json = PlayerPrefs.GetString(Key);
            WalletSaveData data = JsonUtility.FromJson<WalletSaveData>(json);
            if (data == null)
                return new Wallet(startGold, startCash);

            var wallet = new Wallet(data.gold, data.cash);
            if (database != null)
            {
                for (int i = 0; i < data.items.Count; i++)
                {
                    RewardData reward = database.GetById(data.items[i].id);
                    if (reward == null) continue;
                    var instance = new RewardInstance(reward, data.items[i].amount);
                    wallet.Commit(new System.Collections.Generic.List<RewardInstance> { instance });
                }
            }
            return wallet;
        }
    }
}