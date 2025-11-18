using UnityEngine;

namespace Board.Common
{
    public enum Files
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        Count,
    }

    public static class FilesExtensions
    {
        public static Files ToFile(this char file)
        {
            int fileIndex = (file - 'a');
            if (fileIndex < 0 || fileIndex >= (int)Files.Count)
            {
                Debug.LogError($"invalid file text {file}");
                return Files.Count;
            }

            return (Files)fileIndex;
        }

        public static string AsText(this Files file)
        {
            switch (file)
            {
                case Files.A:
                    return "a";
                case Files.B:
                    return "b";
                case Files.C:
                    return "c";
                case Files.D:
                    return "d";
                case Files.E:
                    return "e";
                case Files.F:
                    return "f";
                case Files.G:
                    return "g";
                case Files.H:
                    return "h";
                default:
                    Debug.LogError("Invalid file value: " + file);
                    return "";
            }
        }
    }
}
