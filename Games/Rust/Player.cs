using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Redox.Patcher.Games
{
    public class Player
    {
        internal static void Patch()
        {
            _ = new Player();
        }

        internal Player()
        {
            this.PatchReported();
            this.PatchLanded();
            this.OnKilled();
            this.OnHurt();
            this.OnhealthChange();
            this.OnHostile();
        }

        private void OnHostile()
        {
            TypeDefinition type = Rust.AssemblyCSharp.MainModule.GetType("BasePlayer");
            MethodDefinition hook = Rust.Hooks.Methods.GetMethod("OnPlayerHostile");
            MethodDefinition method = type.Methods.GetMethod("MarkHostileFor");

            const int i = 0;
            ILProcessor processor = method.Body.GetILProcessor();
            var instructions = method.Body.Instructions;

            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Ret));
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Brfalse_S, instructions[1]));
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Call, Rust.AssemblyCSharp.MainModule.ImportReference(hook)));
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Ldarg_1));
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Ldarg_0));
        }

        private void OnhealthChange()
        {
            TypeDefinition type = Rust.AssemblyCSharp.MainModule.GetType("BasePlayer");
            MethodDefinition hook = Rust.Hooks.Methods.GetMethod("OnPlayerHealthChanged");
            MethodDefinition method = type.Methods.GetMethod("OnHealthChanged");

            ILProcessor processor = method.Body.GetILProcessor();
            var instructions = method.Body.Instructions;
            int i = instructions.Count - 1;

            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Call, Rust.AssemblyCSharp.MainModule.ImportReference(hook)));      
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Ldarg_2));
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Ldarg_1));
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Ldarg_0));
            
        }

        private void OnHurt()
        {
            TypeDefinition type = Rust.AssemblyCSharp.MainModule.GetType("BasePlayer");
            MethodDefinition hook = Rust.Hooks.Methods.GetMethod("OnPlayerHurt");
            MethodDefinition method = type.Methods.GetMethod("Hurt");

            const int i = 0; 

            ILProcessor processor = method.Body.GetILProcessor();
            var instructions = method.Body.Instructions;
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Ret));
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Brfalse_S, instructions[1]));
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Call, Rust.AssemblyCSharp.MainModule.ImportReference(hook)));
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Ldarg_1));
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Ldarg_0));

        }

        private void OnKilled()
        {
            TypeDefinition type = Rust.AssemblyCSharp.MainModule.GetType("BasePlayer");
            MethodDefinition hook = Rust.Hooks.Methods.GetMethod("OnPlayerKilled");
            MethodDefinition method = type.Methods.GetMethod("OnKilled");

            ILProcessor processor = method.Body.GetILProcessor();
            var instructions = method.Body.Instructions;
            int i = instructions.Count - 1;
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Call, Rust.AssemblyCSharp.MainModule.ImportReference(hook)));
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Ldarg_1));
            processor.InsertBefore(instructions[i], Instruction.Create(OpCodes.Ldarg_0));
            
        }

        private void PatchLanded()
        {
            TypeDefinition type = Rust.AssemblyCSharp.MainModule.GetType("BasePlayer");
            MethodDefinition hook = Rust.Hooks.Methods.GetMethod("OnPlayerLanded");
            MethodDefinition method = type.Methods.GetMethod("OnPlayerLanded");

            const int i = 0x10;
            ILProcessor processor = method.Body.GetILProcessor();
            var instructions = method.Body.Instructions;
            processor.InsertAfter(instructions[i], Instruction.Create(OpCodes.Call, Rust.AssemblyCSharp.MainModule.ImportReference(hook)));      
            processor.InsertAfter(instructions[i], Instruction.Create(OpCodes.Ldloc_0));
            processor.InsertAfter(instructions[i], Instruction.Create(OpCodes.Ldarg_0));
        }

        private void PatchReported()
        {
            try
            {
                TypeDefinition type = Rust.AssemblyCSharp.MainModule.GetType("BasePlayer");
                MethodDefinition hook = Rust.Hooks.Methods.GetMethod("OnPlayerReported");
                MethodDefinition method = type.Methods.GetMethod("OnPlayerReported");

                const int i = 0x18;

                ILProcessor processor = method.Body.GetILProcessor();
                var instructions = method.Body.Instructions;
              
                processor.InsertAfter(instructions[i], Instruction.Create(OpCodes.Ret));
                processor.InsertAfter(instructions[i], Instruction.Create(OpCodes.Brfalse_S, instructions[26]));  
                processor.InsertAfter(instructions[i], Instruction.Create(OpCodes.Call, Rust.AssemblyCSharp.MainModule.ImportReference(hook)));
                       
                processor.InsertAfter(instructions[i], Instruction.Create(OpCodes.Ldloc_2));
                processor.InsertAfter(instructions[i], Instruction.Create(OpCodes.Ldloc_1));
                processor.InsertAfter(instructions[i], Instruction.Create(OpCodes.Ldloc_3));
                processor.InsertAfter(instructions[i], Instruction.Create(OpCodes.Ldloc_0));
                processor.InsertAfter(instructions[i], Instruction.Create(OpCodes.Ldarg_0));         
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
