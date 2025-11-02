using UnityEngine;
using MoveTrainer.Move;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace MoveTrainer
{
    public class TrainerData
    {
        public enum TrainerType
        {
            DepthFirst,
            BreadthFirst
        }

        public const int Version = 1;

        public bool IsWhiteTrainer = true;
        public int Depth = -1;
        public TrainerType DepthType = TrainerType.DepthFirst;
        public MoveInformation StartingMove;

        public IEnumerable<string> Serialize(TrainerData trainerData)
        {
            yield return TrainerData.Version.ToString();
            yield return (trainerData.IsWhiteTrainer ? "W" : "B");
            yield return trainerData.Depth.ToString();
            yield return trainerData.DepthType.ToString();

            foreach (var line in trainerData.StartingMove.Serialize(0))
            {
                yield return line;
            } 
        }

        public static TrainerData Deserialize(IEnumerator<string> contents)
        {
            TrainerData trainerData = new TrainerData();

            contents.MoveNext();
            Debug.Log(contents.Current);
            int version = int.Parse(contents.Current.Trim());

            contents.MoveNext();
            Debug.Log(contents.Current);
            trainerData.IsWhiteTrainer = contents.Current == "W";

            contents.MoveNext();
            Debug.Log(contents.Current);
            trainerData.Depth = int.Parse(contents.Current.Trim());

            contents.MoveNext();
            Debug.Log(contents.Current);
            trainerData.DepthType = (TrainerType)System.Enum.Parse(typeof(TrainerType), contents.Current.Trim());

            trainerData.StartingMove = new MoveInformation();
            trainerData.StartingMove.Deserialize(contents);
            return trainerData;
        }
    }
}