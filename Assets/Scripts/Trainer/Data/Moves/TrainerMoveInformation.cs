using Board.Common;
using Board.Moves;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Trainer.Data.Moves
{
    public class TrainerMoveInformation
    {
        public TrainerMoveInformation ParentMove = null;
        public List<TrainerMoveInformation> PossibleNextMoves = new List<TrainerMoveInformation>();

        public string MoveNotation;
        public string HintOne;
        public string HintTwo;
        
        public int TimesGuessed;
        public int TimesCorrect;

        public int VariationTimesGuessed;
        public int VariationTimesCorrect;

        public PieceColor Color;

        public override string ToString()
        {
            return MoveNotation;
        }

        public List<TrainerMoveInformation> GetMoveChain()
        {
            List<TrainerMoveInformation> moves = new List<TrainerMoveInformation>();
            TrainerMoveInformation currentMove = this;

            while (currentMove != null)
            {
                moves.Add(currentMove);
                currentMove = currentMove.ParentMove;
            }

            if (moves.Last().MoveNotation == TrainerData.StartingMoveNotation)
            {
                moves.RemoveAt(moves.Count - 1);
            }

            moves.Reverse();

            return moves;
        }

        public IEnumerable<string> Serialize(int depth)
        {
            depth++;
            yield return new string('\t', depth) + "-: ";
            yield return new string('\t', depth) + " hint1: " + HintOne;
            yield return new string('\t', depth) + " hint2: " + HintTwo;
            yield return new string('\t', depth) + " Move: " + MoveNotation;
            yield return new string('\t', depth) + " Count: " + PossibleNextMoves.Count;

            foreach (var nextMove in PossibleNextMoves)
            {
                foreach (var line in nextMove.Serialize(depth))
                {
                    yield return line;
                }
            }
        }

        public void Deserialize(IEnumerator<string> contents)
        {
            contents.MoveNext();
            string firstline = contents.Current.Trim();

            contents.MoveNext();
            string hint1Line = contents.Current.Trim();
            if (hint1Line.StartsWith("hint1: "))
            {
                HintOne = hint1Line.Split(new string[] { "hint1:" }, StringSplitOptions.None)[1].Trim();
            }

            contents.MoveNext();
            string hint2Line = contents.Current.Trim();
            if (hint2Line.StartsWith("hint2: "))
            {
                HintTwo = hint2Line.Split(new string[] { "hint2:" }, StringSplitOptions.None)[1].Trim();
            }

            contents.MoveNext();
            string moveline = contents.Current.Trim();
            if (moveline.StartsWith("Move: "))
            {
                MoveNotation = moveline.Split(new string[] { "Move:" }, StringSplitOptions.None)[1].Trim();
            }
            else
            {
                throw new Exception("Invalid format: Expected Move line.");
            }

            contents.MoveNext();
            string countline = contents.Current.Trim();
            if (countline.StartsWith("Count: "))
            {
                int count = int.Parse(countline.Split(new string[] { "Count:" }, StringSplitOptions.None)[1].Trim());
                for (int i = 0; i < count; i++)
                {
                    TrainerMoveInformation nextMove = new TrainerMoveInformation();
                    nextMove.ParentMove = this;
                    nextMove.Deserialize(contents);
                    PossibleNextMoves.Add(nextMove);
                }
            }
            else
            {
                throw new Exception("Invalid format: Expected Count line.");
            }
        }

    }
}

