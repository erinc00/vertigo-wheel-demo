using System;
using System.Collections.Generic;

namespace Vertigo.Core
{
    [Serializable]
    public sealed class WalletItemSave
    {
        public string id;
        public long amount;
    }

    [Serializable]
    public sealed class WalletSaveData
    {
        public long gold;
        public long cash;
        public List<WalletItemSave> items = new();
    }
}