using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Underlang
{
    public class Program
    {
        public static string CurrentFile = "";
        public static void Main(string[] args)
        {
            try
            {
                CurrentFile = args[0];
                Underlang.Interpret(File.ReadAllText(CurrentFile));
            } catch
            {
                Console.WriteLine("Failed To Load File!");
            }
        }
    }

    public class Underlang
    {
        public static List<string> pointList = new List<string>();
        public static List<string> varNames = new List<string>();
        public static List<string> varValues = new List<string>();
        public static bool InPoint = false;
        public static string CurrentPointData = "";
        public static void Error(string Error, string File, long Line, Exception MoreInfo)
        {
            Console.WriteLine("At Line " + Line + " In " + File + ", " + Error);
            Console.WriteLine("Traceback:\n\n" + MoreInfo.StackTrace + "\n\n" + MoreInfo.Message);
        }
        public static void Error(string Error, string File, long Line)
        {
            Console.WriteLine("At Line " + Line + " In " + File + ", " + Error);
            Console.WriteLine("Traceback:\n\nNone");
        }
        public static void Interpret(string code)
        {
            string[] Instuctions = code.Split('\n');

            for (int i = 0; i < Instuctions.Length; i++)
            {
                string Instuction = Instuctions[i];
                if (Instuction.StartsWith("`"))
                {
                    continue;
                }
                else if (string.IsNullOrEmpty(Instuction))
                {
                    continue;
                }
                else if (Instuction.StartsWith(";point"))
                {
                    string[] vs = Instuction.Split(' ');
                    try
                    {
                        InPoint = true;
                    } catch (Exception e)
                    {
                        Error("Point Initialization Failed", Program.CurrentFile, i, e);
                    }
                }
                else if (Instuction.StartsWith(";endpoint"))
                {
                    InPoint = false;
                    pointList.Add(CurrentPointData);
                }

                else if (Instuction.StartsWith("launchPoint"))
                {
                    string[] vs = Instuction.Split('^');
                    try
                    {
                        if (!InPoint)
                        {
                            Interpret(pointList[int.Parse(vs[1])]);
                        }
                        else
                        {
                            CurrentPointData = CurrentPointData + Instuction + "\n";
                        }
                    }
                    catch (Exception e)
                    {
                        Error("Point Launching Failed", Program.CurrentFile, i, e);
                    }
                }

                else if (Instuction.StartsWith("print"))
                {
                    string[] vs = Instuction.Split('^');
                    string[] vs1 = Instuction.Split('&');
                    try
                    {
                        if (!InPoint)
                        {
                            try
                            {
                                if (vs1.Length != 0)
                                {
                                    Console.WriteLine(varValues[varNames.IndexOf(vs1[1])]);
                                }
                                else
                                {
                                    Console.WriteLine(vs[1]);
                                }
                            } catch
                            {
                                Console.WriteLine(vs[1]);
                            }
                        }
                        else
                        {
                            CurrentPointData = CurrentPointData + Instuction + "\n";
                        }
                    } catch (Exception e)
                    {
                        Error("Print Failed!", Program.CurrentFile, i, e);
                    }
                }

                else if (Instuction.StartsWith("var"))
                {
                    string[] vs = Instuction.Split(' ');
                    string[] value = Instuction.Split('^');
                    try
                    {
                        if (!InPoint)
                        {
                            varNames.Add(vs[1]);
                            varValues.Add(value[1]);
                        }
                        else
                        {
                            CurrentPointData = CurrentPointData + Instuction + "\n";
                        }
                    }
                    catch (Exception e)
                    {
                        Error("Var Initialization Failed!", Program.CurrentFile, i, e);
                    }
                }

                else if (Instuction.StartsWith("increment"))
                {
                    string[] vs = Instuction.Split('^');
                    try
                    {
                        if (!InPoint)
                        {
                            varValues[varNames.IndexOf(vs[1])] = (int.Parse(varValues[varNames.IndexOf(vs[1])]) + 1).ToString();
                        } else
                        {
                            CurrentPointData = CurrentPointData + Instuction + "\n";
                        }
                    } catch (Exception e)
                    {
                        Error("Incrementing Failed!", Program.CurrentFile, i, e);
                    }
                }

                else if (Instuction.StartsWith("if~bool"))
                {
                    string[] vs = Instuction.Split('^');
                    try
                    {
                        if (!InPoint)
                        {
                            if (varValues[varNames.IndexOf(vs[1])] == "true")
                            {
                                Interpret(pointList[int.Parse(vs[2])]);
                            }
                        }
                        else
                        {
                            CurrentPointData = CurrentPointData + Instuction + "\n";
                        }
                    } catch (Exception e)
                    {
                        Error("Failed To Find In Boolean Is True!", Program.CurrentFile, i, e);
                    }
                }
                else if (Instuction.StartsWith("if!bool"))
                {
                    string[] vs = Instuction.Split('^');
                    try
                    {
                        if (!InPoint)
                        {
                            if (varValues[varNames.IndexOf(vs[1])] == "false")
                            {
                                Interpret(pointList[int.Parse(vs[2])]);
                            }
                        }
                        else
                        {
                            CurrentPointData = CurrentPointData + Instuction + "\n";
                        }
                    }
                    catch (Exception e)
                    {
                        Error("Failed To Find In Boolean Is False!", Program.CurrentFile, i, e);
                    }
                }
                else if (Instuction.StartsWith("if~match"))
                {
                    string[] vs = Instuction.Split('^');
                    try
                    {
                        if (!InPoint)
                        {
                            if (varValues[varNames.IndexOf(vs[1])] == varValues[varNames.IndexOf(vs[2])])
                            {
                                Interpret(pointList[int.Parse(vs[3])]);
                            }
                        }
                        else
                        {
                            CurrentPointData = CurrentPointData + Instuction + "\n";
                        }
                    }
                    catch (Exception e)
                    {
                        Error("Failed To Find If Items Don't Match!", Program.CurrentFile, i, e);
                    }
                }
                else if (Instuction.StartsWith("if!match"))
                {
                    string[] vs = Instuction.Split('^');
                    try
                    {
                        if (!InPoint)
                        {
                            if (varValues[varNames.IndexOf(vs[1])] != varValues[varNames.IndexOf(vs[2])])
                            {
                                Interpret(pointList[int.Parse(vs[3])]);
                            }
                        }
                        else
                        {
                            CurrentPointData = CurrentPointData + Instuction + "\n";
                        }
                    }
                    catch (Exception e)
                    {
                        Error("Failed To Find If Items Match!", Program.CurrentFile, i, e);
                    }
                }

                else if (Instuction.StartsWith("readFile"))
                {
                    string[] vs = Instuction.Split('^');
                    try
                    {
                        if (!InPoint)
                        {
                            varValues[varNames.IndexOf(vs[2])] = File.ReadAllText(vs[1]);
                        }
                        else
                        {
                            CurrentPointData = CurrentPointData + Instuction + "\n";
                        }
                    }
                    catch (Exception e)
                    {
                        Error("Failed To Read File!", Program.CurrentFile, i, e);
                    }
                }

                else if (Instuction.StartsWith("writeFile"))
                {
                    string[] vs = Instuction.Split('^');
                    try
                    {
                        if (!InPoint)
                        {
                            File.WriteAllText(vs[1], vs[2]);
                        }
                        else
                        {
                            CurrentPointData = CurrentPointData + Instuction + "\n";
                        }
                    }
                    catch (Exception e)
                    {
                        Error("Failed To Write File!", Program.CurrentFile, i, e);
                    }
                }

                else if (Instuction.StartsWith("halt"))
                {
                    string[] vs = Instuction.Split('^');
                    if (!InPoint)
                    {
                        System.Environment.Exit(int.Parse(vs[1]));
                    }
                    else
                    {
                        CurrentPointData = CurrentPointData + Instuction + "\n";
                    }
                }

                else if (Instuction.StartsWith("calc"))
                {
                    string[] vs = Instuction.Split("^");
                    try
                    {
                        if (!InPoint)
                        {
                            if (vs[2] == "+")
                            {
                                varValues[varNames.IndexOf(vs[4])] = (int.Parse(vs[1]) + int.Parse(vs[3])).ToString();
                            }
                            else if (vs[2] == "-")
                            {
                                varValues[varNames.IndexOf(vs[4])] = (int.Parse(vs[1]) - int.Parse(vs[3])).ToString();
                            }
                            else if (vs[2] == "/")
                            {
                                varValues[varNames.IndexOf(vs[4])] = (int.Parse(vs[1]) / int.Parse(vs[3])).ToString();
                            }
                            else if (vs[2] == "*")
                            {
                                varValues[varNames.IndexOf(vs[4])] = (int.Parse(vs[1]) * int.Parse(vs[3])).ToString();
                            }
                            else if (vs[2] == "%")
                            {
                                varValues[varNames.IndexOf(vs[4])] = (int.Parse(vs[1]) % int.Parse(vs[3])).ToString();
                            }
                            else if (vs[2] == "/\\")
                            {
                                varValues[varNames.IndexOf(vs[4])] = (int.Parse(vs[1]) ^ int.Parse(vs[3])).ToString();
                            }
                        }
                        else
                        {
                            CurrentPointData = CurrentPointData + Instuction + "\n";
                        }
                    } catch (Exception e)
                    {
                        Error("Calculation Failed!", Program.CurrentFile, i, e);
                    }
                }
            }
        }
    }
}
