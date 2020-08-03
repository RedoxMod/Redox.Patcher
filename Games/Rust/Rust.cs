using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Redox.Patcher.Games
{
    public class Rust
    {
        internal static AssemblyDefinition RedoxCore;
        internal static AssemblyDefinition RedoxRust;
        internal static AssemblyDefinition AssemblyCSharp;
        internal static AssemblyDefinition FacepunchNetwork;

        internal static TypeDefinition Hooks;

        public void Patch()
        {
            Console.WriteLine("Patching rust..");
            try
            {
                Console.WriteLine("Reading assemblies..");
                //Lets read all the assemblies we are going to patch.
                RedoxCore = AssemblyDefinition.ReadAssembly("Redox.dll");
                RedoxRust = AssemblyDefinition.ReadAssembly("Redox.Rust.dll");
                AssemblyCSharp = AssemblyDefinition.ReadAssembly("Assembly-CSharp.dll");
                FacepunchNetwork = AssemblyDefinition.ReadAssembly("Facepunch.Network.dll");
                Hooks = RedoxRust.MainModule.GetType("Redox.Rust.Hooks");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to load assemblies. " + ex.Message);
                Console.Read();
            }
            Console.WriteLine("Starting to patch..");
            PatchBootstrap();
            PatchPlayerConnected();
            PatchPlayerDisconnected();
            Player.Patch();
            AssemblyCSharp.Write("Patched\\Assembly-CSharp.dll");
            FacepunchNetwork.Write("Patched\\Facepunch.Network.dll");
            Console.WriteLine("The patch was ended successfully.");
            Console.Read();
        }

        private void PatchPlayerDisconnected()
        {
            try
            {
                Console.WriteLine("Patching OnDisconnect..");
                TypeDefinition networkServer = FacepunchNetwork.MainModule.GetType("Network.Server");

                MethodDefinition definition = Hooks.Methods.GetMethod("OnPlayerDisconnect");
                MethodDefinition definition1 = networkServer.Methods.GetMethod("OnDisconnected");

                int i = definition1.Body.Instructions.Count - 1;
                ILProcessor processor = definition1.Body.GetILProcessor();
                processor.InsertBefore(definition1.Body.Instructions[i], Instruction.Create(OpCodes.Call, FacepunchNetwork.MainModule.ImportReference(definition)));
                processor.InsertBefore(definition1.Body.Instructions[i], Instruction.Create(OpCodes.Ldarg_2));
                processor.InsertBefore(definition1.Body.Instructions[i], Instruction.Create(OpCodes.Ldarg_1));
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception has thrown while trying to patch \"OnDisconnect\". Error" + ex.Message);
            }
        }

        private void PatchPlayerConnected()
        {
            try
            {                                                   
                Console.WriteLine("Patching PlayerConnection..");
                TypeDefinition player = AssemblyCSharp.MainModule.GetType("BasePlayer");

                MethodDefinition definition = Hooks.Methods.GetMethod("OnPlayerConnection");
                MethodDefinition definition1 = player.Methods.GetMethod("PlayerInit");

                ILProcessor processor = definition1.Body.GetILProcessor();
                const int i = 0x85;
                processor.InsertAfter(definition1.Body.Instructions[i], Instruction.Create(OpCodes.Call, AssemblyCSharp.MainModule.ImportReference(definition)));
                processor.InsertAfter(definition1.Body.Instructions[i], Instruction.Create(OpCodes.Ldarg_0));
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception has thrown while trying to patch \"OnNewConnection\". Error" + ex.Message);
            }
        }

        private void PatchBootstrap()
        {
            try
            {
                Console.WriteLine("Patching bootstrap..");
                TypeDefinition bootstrap = RedoxCore.MainModule.GetType("Redox.Bootstrap");
                TypeDefinition serverBootstrap = AssemblyCSharp.MainModule.GetType("Bootstrap");

                MethodDefinition redoxInit = bootstrap.Methods.GetMethod("Init");
                MethodDefinition serverInit = serverBootstrap.Methods.GetMethod("StartupShared");

                ILProcessor processor = serverInit.Body.GetILProcessor();
                processor.InsertBefore(serverInit.Body.Instructions[0], Instruction.Create(OpCodes.Call, AssemblyCSharp.MainModule.ImportReference(redoxInit)));
                processor.InsertBefore(serverInit.Body.Instructions[0], Instruction.Create(OpCodes.Ldstr, ""));
            }
            catch(Exception ex)
            {
                Console.WriteLine("An exception has thrown while trying to patch \"Bootstrap\". Error" + ex.Message);
            }
        }
    }
}
