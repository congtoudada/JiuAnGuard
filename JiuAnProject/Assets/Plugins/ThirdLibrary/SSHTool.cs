using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Renci.SshNet;

public class SSHTool
{
    public delegate void GetSSHLog(bool isOK, string log);
    public static event GetSSHLog getSSHLog;
        
    /// <summary>
    /// SSH登录远程Linux服务器，并运行指令
    /// </summary>
    /// <param name="host">远程Linux服务器IP或域名</param>
    /// <param name="username">账号名</param>
    /// <param name="password">账号密码</param>
    /// <param name="command">命令</param>
    /// <returns></returns>
    public static void RunSSHCommands(string host, string username, string password, string command)
    {
        if (string.IsNullOrEmpty(command))
        {
            getSSHLog?.Invoke(false, "指令为空!");
        }
        try
        {
            using (var client = new SshClient(host, username, password))
            {
                try
                {
                    client.Connect();
                    if (command != null)
                    {
                        var sshCommond = client.RunCommand(command);
                        sshCommond.Execute();
                        getSSHLog?.Invoke(true, sshCommond.Result);
                    }

                    client.Disconnect();
                }
                catch (Exception e)
                {
                    getSSHLog?.Invoke(false, e.Message);
                }
            }
        }
        catch (Exception e)
        {
            getSSHLog?.Invoke(false, e.Message);
        }
    }
    
    // /// <summary>
    // /// SSH登录远程Linux服务器，并运行指令
    // /// </summary>
    // /// <param name="host">远程Linux服务器IP或域名</param>
    // /// <param name="username">账号名</param>
    // /// <param name="password">账号密码</param>
    // /// <param name="command">命令</param>
    // /// <returns></returns>
    // public static async UniTaskVoid RunSSHCommandsAsync(string host, string username, string password, string command)
    // {
    //     if (string.IsNullOrEmpty(command))
    //     {
    //         getSSHLog?.Invoke("指令为空!");
    //     }
    //     try
    //     {
    //         using (var client = new SshClient(host, username, password))
    //         {
    //             try
    //             {
    //                 await client.ConnectAsync(default(CancellationToken));
    //                 if (command != null)
    //                 {
    //                     var sshCommond = client.RunCommand(command);
    //                     await sshCommond.ExecuteAsync();
    //                     getSSHLog?.Invoke(sshCommond.Result);
    //                 }
    //
    //                 client.Disconnect();
    //             }
    //             catch (Exception e)
    //             {
    //                 getSSHLog?.Invoke(e.Message);
    //             }
    //         }
    //     }
    //     catch (Exception e)
    //     {
    //         getSSHLog?.Invoke(e.Message);
    //     }
    // }
    //
}