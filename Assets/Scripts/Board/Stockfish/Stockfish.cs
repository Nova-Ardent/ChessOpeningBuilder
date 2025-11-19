using Board.Evaluation;
using Board.History.TopMove;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System;

namespace Board.Stockfish
{
    public class StockfishObject : MonoBehaviour
    {
        [SerializeField] String _filePath;

        [SerializeField] EvalBar _evalBar;
        [SerializeField] TopMoveList _topMoveList;

        public bool LastTurnWasWhite { get; private set; }
        public bool HasMate { get; private set; }
        public int MateIndex { get; private set; }
        public float PositionEval { get; private set; }

        Regex centipawnMatch = new Regex("cp [+-]?\\d+(?:\\.\\d+)?");
        Regex mateMatch = new Regex("mate [+-]?\\d+(?:\\.\\d+)?");
        Regex multiPV = new Regex("multipv \\d+");
        Regex depth = new Regex("depth \\d+");


        Process stockFishProcess;

        void Awake()
        {
            string path = Application.streamingAssetsPath + "\\Stockfish\\stockfish\\stockfish-windows-x86-64-avx2.exe";

            ProcessStartInfo launchInfo = new ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            stockFishProcess = new Process();
            stockFishProcess.StartInfo = launchInfo;
            stockFishProcess.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputDataReceived);
            if (stockFishProcess.Start())
            {
                stockFishProcess.BeginErrorReadLine();
                stockFishProcess.BeginOutputReadLine();

                // Send start commands to stockfish
                SendLine("uci");
                SendLine("isready");
                SendLine("ucinewgame");
            }
            else
            {
                UnityEngine.Debug.LogError("stockfish failed to start."); 
            }
        }

        public void OnApplicationQuit()
        {
            stockFishProcess?.Close();
            stockFishProcess?.Dispose();
        }

        private void OnDestroy()
        {
            stockFishProcess?.Close();
            stockFishProcess?.Dispose();
        }

        void SendLine(string command)
        {
            try
            {
                stockFishProcess.StandardInput.WriteLine(command);
                stockFishProcess.StandardInput.Flush();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Failed to send command to stockfish: " + e.Message);
                UnityEngine.Debug.LogError("Command was: " + command);
            }
        }

        public void GoToPosition(string fen, bool isWhite)
        {
            LastTurnWasWhite = isWhite;

            SendLine("stop");
            _topMoveList.ClearData();

            SendLine("position fen " + fen);
            SendLine("setoption name multipv value 5");
            SendLine("go infinite");
        }

        public void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Get data from event
            string data = e.Data;
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            Match valueMatch = centipawnMatch.Match(data);
            Match mateValueMatch = mateMatch.Match(data);
            Match multiPVMatch = multiPV.Match(data);
            Match depthMatch = depth.Match(data);

            int depthValue = -1;
            if (depthMatch.Success)
            {
                string depthData = depthMatch.Groups.First().Value;
                depthData = depthData.Split(" ")[1].Trim();

                if (int.TryParse(depthData, out depthValue))
                { 
                }
            }
            
            int multiPVValue = -1;
            if (multiPVMatch.Success)
            {
                string multiPVData = multiPVMatch.Groups.First().Value;
                multiPVData = multiPVData.Split(" ")[1].Trim();

                if (int.TryParse(multiPVData, out multiPVValue))
                {
                }
            }

            string pvData = "";
            if (data.Contains("pv"))
            {
                pvData = data.Split("pv").Last().Trim();
                if (pvData.Contains(" "))
                {
                    pvData = pvData.Split(" ").First();
                }
            }

            _evalBar.HasMate = false;
            HasMate = false;

            if (mateValueMatch.Success)
            {
                string mateData = mateValueMatch.Groups.First().Value;
                mateData = mateData.Split(" ")[1].Trim();

                int mateValue = 0;
                if (int.TryParse(mateData, out mateValue))
                {
                    if (LastTurnWasWhite)
                    {
                        MateIndex = mateValue;
                        HasMate = true;
                    }
                    else
                    {
                        MateIndex = -mateValue;
                        HasMate = true;
                    }

                    if (multiPVValue == 1)
                    {
                        _evalBar.MateValue = MateIndex;
                        _evalBar.HasMate = true;
                    }

                    if (multiPVValue != -1 && depthValue != -1)
                    {
                        _topMoveList.SetTopMoveData
                            ( multiPVValue
                            , new TopMoveData()
                                { Depth = depthValue
                                , Evaluation = 0
                                , HasMate = true
                                , MateIn = MateIndex
                                , UCI = pvData
                                }
                            );
                    }
                }
            }
            else if (valueMatch.Success)
            {
                string centiPawnData = valueMatch.Groups.First().Value;
                centiPawnData = centiPawnData.Split(" ")[1].Trim();

                int centipawns = 0;

                if (int.TryParse(centiPawnData, out centipawns))
                {
                    if (_evalBar != null)
                    {
                        if (LastTurnWasWhite)
                        {
                            PositionEval = centipawns / 100f;
                        }
                        else
                        {
                            PositionEval = -centipawns / 100f;
                        }

                        if (multiPVValue == 1)
                        {
                            _evalBar.EvalAmount = PositionEval;
                        }

                        if (multiPVValue != -1 && depthValue != -1)
                        {
                            _topMoveList.SetTopMoveData
                                ( multiPVValue
                                , new TopMoveData()
                                    { Depth = depthValue
                                    , Evaluation = PositionEval
                                    , HasMate = false
                                    , MateIn = 0
                                    , UCI = pvData
                                    }
                                );
                        }
                    }
                }
            }

        }
    }
}
