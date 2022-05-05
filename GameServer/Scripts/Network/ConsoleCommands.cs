using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ConsoleCommands : MonoBehaviour
{

    private string inputUser = "";
    // Start is called before the first frame update
    void Start()
    {
        Task.Factory.StartNew(() =>
        {
            while (true)
            {
                inputUser = Console.ReadLine();
            }
        });
      
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(inputUser))
        {
            string[] args = inputUser.Split(' ');
            if (Commands.isCommand(args[0]))
            {
                if (args[0].Equals("kick"))
                {
                    if (args.Length >= 2)
                    {
                        if (string.IsNullOrEmpty(args[1]))
                        {
                            Console.WriteLine("Incorrect syntax: kick <player> <reason>");
                            inputUser = "";
                            return;
                        }
                        Commands.KickPlayer(args[1], args[2]);
                    } else
                    {
                        Console.WriteLine("Incorrect syntax: kick <player> <reason>");
                    }
                } else if (args[0].Equals("move"))
                {
                    if (args.Length >= 4)
                    {
                        if (string.IsNullOrEmpty(args[1]) || string.IsNullOrEmpty(args[2]) || string.IsNullOrEmpty(args[3]) || string.IsNullOrEmpty(args[4]))
                        {
                            Console.WriteLine("Incorrect syntax: move <player> <x> <y> <z>");
                            inputUser = "";
                            return;
                        }
                        Commands.MovePlayer(args[1], float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]));
                    } else
                    {
                        Console.WriteLine("Incorrect syntax: move <player> <x> <y> <z>");
                    }
                } else if (args[0].Equals("spawnZombieAt"))
                {
                    if (args.Length >= 2)
                    {
                        if (string.IsNullOrEmpty(args[1]) || string.IsNullOrEmpty(args[2]))
                        {
                            Console.WriteLine("Incorrect syntax: spawnZombieAt <player> <zombieID>");
                            inputUser = "";
                            return;
                        }
                        Commands.SpawnZombieAt(args[1], int.Parse(args[2]));
                    } else
                    {
                        Console.WriteLine("Incorrect syntax: spawnZombieAt <player> <zombieID>");
                    }
                } else if (args[0].Equals("spawnZombie"))
                {
                    if (args.Length >= 4)
                    {
                        if (string.IsNullOrEmpty(args[1]) || string.IsNullOrEmpty(args[2]) || string.IsNullOrEmpty(args[3]) || string.IsNullOrEmpty(args[4]))
                        {
                            Console.WriteLine("Incorrect syntax: spawnZombie <x> <y> <z> <zombieID>");
                            inputUser = "";
                            return;
                        }
                            Commands.SpawnZombie(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]), int.Parse(args[4]));
                    } else
                    {
                        Console.WriteLine("Incorrect syntax: spawnZombie <x> <y> <z> <zombieID>");
                    }
                } else if (args[0].Equals("spawnHeli"))
                {
                    if (args.Length >= 5)
                    {
                        if (string.IsNullOrEmpty(args[1]) || string.IsNullOrEmpty(args[2]) || string.IsNullOrEmpty(args[3]) || string.IsNullOrEmpty(args[4]) || string.IsNullOrEmpty(args[5]))
                        {
                            Console.WriteLine("Incorrect syntax: spawnHeli <x> <y> <z> <toPlayer> <speed>");
                            inputUser = "";
                            return;
                        }
                            Commands.SpawnHeli(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]), args[4], float.Parse(args[5]));
                    } else
                    {
                        Console.WriteLine("Incorrect syntax: spawnHeli <x> <y> <z> <toPlayer> <speed>");
                    }
                } else if (args[0].Equals("setTag"))
                {
                    if (args.Length >= 2)
                    {
                        if (string.IsNullOrEmpty(args[1]) || string.IsNullOrEmpty(args[2]))
                        {
                            Console.WriteLine("Incorrect syntax: setTag <player> <tag>");
                            inputUser = "";
                            return;
                        }
                            Commands.SetTag(args[1], args[2]);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect syntax: setTag <player> <tag>");
                    }
                } else if (args[0].Equals("setColor"))
                {
                    if (args.Length >= 2)
                    {
                        if (string.IsNullOrEmpty(args[1]) || string.IsNullOrEmpty(args[2]))
                        {
                            Console.WriteLine("Incorrect syntax: setColor <player> <color>");
                            inputUser = "";
                            return;
                        }
                            Commands.SetColor(args[1], args[2]);
                    } else
                    {
                        Console.WriteLine("Incorrect syntax: setColor <player> <color>");
                    }
                }
                inputUser = "";
            }
        }
    }
}
