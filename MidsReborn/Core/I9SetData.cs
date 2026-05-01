using System;
using System.Linq;

namespace Mids_Reborn.Core
{
    public class I9SetData
    {
        public int PowerIndex;
        public sSetInfo[] SetInfo = new sSetInfo[0];

        public I9SetData()
        {
        }

        public I9SetData(I9SetData iSd)
        {
            PowerIndex = iSd.PowerIndex;
            SetInfo = new sSetInfo[iSd.SetInfo.Length];
            for (var index = 0; index <= SetInfo.Length - 1; ++index)
            {
                SetInfo[index].SetIDX = iSd.SetInfo[index].SetIDX;
                SetInfo[index].SlottedCount = iSd.SetInfo[index].SlottedCount;
                SetInfo[index].Powers = new int[iSd.SetInfo[index].Powers.Length];
                Array.Copy(iSd.SetInfo[index].Powers, SetInfo[index].Powers, iSd.SetInfo[index].Powers.Length);
                SetInfo[index].EnhIndexes = new int[iSd.SetInfo[index].EnhIndexes.Length];
                Array.Copy(iSd.SetInfo[index].EnhIndexes, SetInfo[index].EnhIndexes, iSd.SetInfo[index].EnhIndexes.Length);
                var pieceIndexes = iSd.SetInfo[index].PieceIndexes ?? Array.Empty<int>();
                SetInfo[index].PieceIndexes = new int[pieceIndexes.Length];
                Array.Copy(pieceIndexes, SetInfo[index].PieceIndexes, pieceIndexes.Length);
            }
        }

        public bool Empty => SetInfo.Length < 1;

        public void Add(ref I9Slot iEnh)
        {
            if (iEnh.Enh < 0 || DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID != Enums.eType.SetO)
                return;
            var nIdSet = DatabaseAPI.Database.Enhancements[iEnh.Enh].nIDSet;
            var pieceIndex = DatabaseAPI.TryGetSetPieceIndexForEnhancement(iEnh.Enh, out _, out var resolvedPieceIndex)
                ? resolvedPieceIndex
                : -1;
            var index = Lookup(nIdSet);
            if (index >= 0)
            {
                Array.Resize(ref SetInfo[index].EnhIndexes, SetInfo[index].EnhIndexes.Length + 1);
                SetInfo[index].EnhIndexes[^1] = iEnh.Enh;
                SetInfo[index].PieceIndexes ??= Array.Empty<int>();
                Array.Resize(ref SetInfo[index].PieceIndexes, SetInfo[index].PieceIndexes.Length + 1);
                SetInfo[index].PieceIndexes[^1] = pieceIndex;
                SetInfo[index].SlottedCount = SetInfo[index].PieceIndexes.Where(piece => piece >= 0).Distinct().Count();
            }
            else
            {
                Array.Resize(ref SetInfo, SetInfo.Length + 1);
                SetInfo[^1].SetIDX = nIdSet;
                SetInfo[^1].SlottedCount = pieceIndex >= 0 ? 1 : 0;
                SetInfo[^1].Powers = new int[0];
                Array.Resize(ref SetInfo[^1].EnhIndexes, 1);
                SetInfo[^1].EnhIndexes[^1] = iEnh.Enh;
                SetInfo[^1].PieceIndexes = new[] { pieceIndex };
            }
        }

        private int Lookup(int setID)

        {
            int num;
            if (setID < 0)
            {
                num = -1;
            }
            else
            {
                for (var index = 0; index <= SetInfo.Length - 1; ++index)
                    if (SetInfo[index].SetIDX == setID)
                        return index;
                num = -1;
            }

            return num;
        }

        public void BuildEffects(Enums.ePvX pvMode)
        {
            for (var index1 = 0; index1 <= SetInfo.Length - 1; ++index1)
            {
                if (SetInfo[index1].SlottedCount > 1)
                    for (var index2 = 0;
                         index2 <= DatabaseAPI.Database.EnhancementSets[SetInfo[index1].SetIDX].Bonus.Length - 1;
                         ++index2)
                    {
                        if (!((DatabaseAPI.Database.EnhancementSets[SetInfo[index1].SetIDX].Bonus[index2].Slotted <=
                               SetInfo[index1].SlottedCount) &
                              ((DatabaseAPI.Database.EnhancementSets[SetInfo[index1].SetIDX].Bonus[index2].PvMode ==
                                pvMode) |
                               (DatabaseAPI.Database.EnhancementSets[SetInfo[index1].SetIDX].Bonus[index2].PvMode ==
                                Enums.ePvX.Any))))
                            continue;
                        for (var index3 = 0;
                             index3 <= DatabaseAPI.Database.EnhancementSets[SetInfo[index1].SetIDX].Bonus[index2].Index
                                 .Length - 1;
                             ++index3)
                        {
                            Array.Resize(ref SetInfo[index1].Powers, SetInfo[index1].Powers.Length + 1);
                            SetInfo[index1].Powers[^1] = DatabaseAPI.Database
                                .EnhancementSets[SetInfo[index1].SetIDX].Bonus[index2].Index[index3];
                        }
                    }

                if (SetInfo[index1].SlottedCount <= 0)
                    continue;

                var distinctPieceIndexes = (SetInfo[index1].PieceIndexes ?? Array.Empty<int>())
                    .Where(pieceIndex => pieceIndex >= 0)
                    .Distinct()
                    .ToArray();

                foreach (var pieceIndex in distinctPieceIndexes)
                {
                    var rawMemberPosition = DatabaseAPI.GetSpecialRawMemberPositionForSetPiece(SetInfo[index1].SetIDX, pieceIndex);
                    if (rawMemberPosition < 0)
                    {
                        continue;
                    }

                    for (var powerIndex = 0;
                         powerIndex < DatabaseAPI.Database.EnhancementSets[SetInfo[index1].SetIDX].SpecialBonus[rawMemberPosition].Index.Length;
                         ++powerIndex)
                    {
                        Array.Resize(ref SetInfo[index1].Powers, SetInfo[index1].Powers.Length + 1);
                        SetInfo[index1].Powers[^1] = DatabaseAPI.Database
                            .EnhancementSets[SetInfo[index1].SetIDX].SpecialBonus[rawMemberPosition].Index[powerIndex];
                    }
                }
            }
        }

        public struct sSetInfo
        {
            public int SetIDX;
            public int SlottedCount;
            public int[] Powers;
            public int[] EnhIndexes;
            public int[] PieceIndexes;
        }
    }
}
