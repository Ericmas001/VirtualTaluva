using System.Collections.Generic;
using System.Linq;
using Com.Ericmas001.Common;
using VirtualTaluva.Demo.Enums;

namespace VirtualTaluva.Demo.Models
{
    public class LandDealer
    {
        private readonly IBoard m_Board;
        private readonly int m_NbCards;

        private class LandDistribution
        {
            /*
                 48 Volcano tiles
                    _
                  _/V\_
                 /D   L\ =====> Left:Desert * Righ:Lagoon
                 \_/-\_/

                Distribution of Terrain, keeping left and right distinct
                (assuming tiles are held with the volcano at top)

                 Right: Jungle  Grass   Desert  Quarry  Lagoon
                Left:
                Jungle    1       6       4       2       2
                Grass     5       1       2       2       1
                Desert    4       2       1       2       1
                Quarry    2       2       1       1       1
                Lagoon    1       1       1       1       1

                Total    28      23      19      15      11   = 96


                Distribution of Terrain, position independent

                        Jungle  Grass   Desert  Quarry  Lagoon
                Jungle    1       11      8       4       3
                Grass              1      4       4       2
                Desert                    1       3       2
                Quarry                            1       2
                Lagoon                                    1

                https://boardgamegeek.com/thread/184290/terrain-distribution
             */
            public LandEnum LeftLand { get; }
            public LandEnum RightLand { get; }
            public int Nb { get; }

            public LandDistribution(LandEnum leftLand, LandEnum rightLand, int nb)
            {
                LeftLand = leftLand;
                RightLand = rightLand;
                Nb = nb;
            }
        }

        private readonly LandDistribution[] m_Distribution = {
            new LandDistribution(LandEnum.Jungle, LandEnum.Jungle, 1),
            new LandDistribution(LandEnum.Jungle, LandEnum.Grass, 6),
            new LandDistribution(LandEnum.Jungle, LandEnum.Desert, 4),
            new LandDistribution(LandEnum.Jungle, LandEnum.Quarry, 2),
            new LandDistribution(LandEnum.Jungle, LandEnum.Lagoon, 2),
            new LandDistribution(LandEnum.Grass, LandEnum.Jungle, 5),
            new LandDistribution(LandEnum.Grass, LandEnum.Grass, 1),
            new LandDistribution(LandEnum.Grass, LandEnum.Desert, 2),
            new LandDistribution(LandEnum.Grass, LandEnum.Quarry, 2),
            new LandDistribution(LandEnum.Grass, LandEnum.Lagoon, 1),
            new LandDistribution(LandEnum.Desert, LandEnum.Jungle, 4),
            new LandDistribution(LandEnum.Desert, LandEnum.Grass, 3),
            new LandDistribution(LandEnum.Desert, LandEnum.Desert, 1),
            new LandDistribution(LandEnum.Desert, LandEnum.Quarry, 2),
            new LandDistribution(LandEnum.Desert, LandEnum.Lagoon, 1),
            new LandDistribution(LandEnum.Quarry, LandEnum.Jungle, 2),
            new LandDistribution(LandEnum.Quarry, LandEnum.Grass, 2),
            new LandDistribution(LandEnum.Quarry, LandEnum.Desert, 1),
            new LandDistribution(LandEnum.Quarry, LandEnum.Quarry, 1),
            new LandDistribution(LandEnum.Quarry, LandEnum.Lagoon, 1),
            new LandDistribution(LandEnum.Lagoon, LandEnum.Jungle, 1),
            new LandDistribution(LandEnum.Lagoon, LandEnum.Grass, 1),
            new LandDistribution(LandEnum.Lagoon, LandEnum.Desert, 1),
            new LandDistribution(LandEnum.Lagoon, LandEnum.Quarry, 1),
            new LandDistribution(LandEnum.Lagoon, LandEnum.Lagoon, 1),
        };
        private Stack<PlayingTile> Deck { get; set; }

        public PlayingTile DealTile()
        {
            return Deck.Pop();
        }

        public void FreshDeck()
        {
            Deck = GetShuffledDeck();
        }

        private Stack<PlayingTile> GetShuffledDeck()
        {
            var deck = new Stack<PlayingTile>();
            var remaining = GetSortedDeck();
            while (remaining.Count > 48 - m_NbCards)
            {
                var id = RandomUtil.RandomWithMax(remaining.Count - 1);
                deck.Push(remaining[id]);
                remaining.RemoveAt(id);
            }
            return deck;
        }

        private List<PlayingTile> GetSortedDeck()
        {
            return m_Distribution.SelectMany(x => Enumerable.Range(0, x.Nb).Select(y => new PlayingTile(m_Board, x.LeftLand, x.RightLand))).ToList();
        }

        public LandDealer(IBoard board, int nbCards)
        {
            m_Board = board;
            m_NbCards = nbCards;
        }
    }
}
