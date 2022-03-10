using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketTanuki
{
    public class Zobrist
    {
        public long[,,] PieceSquare { get; } = new long[(int)Piece.NumPieces, Position.BoardSize, Position.BoardSize];
        public long[] HandPiece { get; } = new long[(int)Piece.NumPieces];
        public long Side { get; set; }
        public static Zobrist Instance { get; } = new Zobrist();

        public Zobrist()
        {
            var random = new Random(0);
            for (int pieceIndex = 0; pieceIndex < (int)Piece.NumPieces; ++pieceIndex)
            {
                for (int file = 0; file < Position.BoardSize; ++file)
                {
                    for (int rank = 0; rank < Position.BoardSize; ++rank)
                    {
                        PieceSquare[pieceIndex, file, rank] = GetRandomValue(random);
                    }
                }
            }

            for (int handPieceIndex = 0; handPieceIndex < (int)Piece.NumPieces; ++handPieceIndex)
            {
                HandPiece[handPieceIndex] = GetRandomValue(random);
            }

            Side = 1;
        }

        private static long GetRandomValue(Random random)
        {
			var buf = new byte[8];
			random.NextBytes(buf);
            // 最下位のビットを落とさないと、Sideと被り、意図しない計算結果となる。
			return BitConverter.ToInt64(buf, 0) & 0xffffffffffffe;
        }
    }
}
