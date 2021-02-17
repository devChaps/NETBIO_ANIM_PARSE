using System;
using System.IO;





namespace NETBIO_ANIM_PARSE
{
    public class Program
    {



        public struct AHI_HEADER_OBJ
        {
            public float ufloat00;
            public Int32 Bone_Count;
            public Int32 t_size;
            public Int32 uint00;
            public Int32 uint01;
            public Int32 count00;
            public Int32 uint02;
        }


        public struct AHI_BONE_OBJP
        {
            public Int32 Obj_id;
            public Int32 Uint00; // always 1?
            public Int32 bone_size; // 268?
            public Int32 bone_id;
            public Int32 bone_prev;
            public Int32 bone_next;
            public Int32 bone_obj;
            public float[] scale;
            public float ufloat00;
            public float[] QNon;
            public float[] vec3_transform;
            public float t4;

        }








        public struct ANIM_BLOCK_HEADER_OBJ
        {
            public Int32 block_ID; // always seems to be 0x02000080
            public Int32 sub_count; // # of sub blocks in the entire anim block
            public Int32 blk_sz; // total size of entire anim block

        }


        // the header for each idv sub block
        public struct ANIM_SUB_BLOCK_HEADER_OBJ
        {
            public Int32 subBLockID; // always seems to be 0x3F000080
            public Int32 instruction_count; // # of instructions inside sub chunk
            public Int32 sz; // size of the sub block

        }


        public struct Instruction_obj_header
        {
            public byte id;
            public byte ubyte00;
            public byte instruction_type;
            public byte t_frame;
            public Int32 count; // frame count?
            public Int32 blk_sz; // total size of the entire instruction block
        }



        // 8 byte instructions per frame count
        public struct Instruction_Obj
        {
            public Int16 Rotation00;
            public Int16 Rotation01;
            public Int16 Rotation02;
            public Int16 Rotation03;
          
        }




        public static ANIM_BLOCK_HEADER_OBJ ANIM_BLOCK_HEADER = new ANIM_BLOCK_HEADER_OBJ();
        public static ANIM_SUB_BLOCK_HEADER_OBJ[] ANIM_SUB_HEADER = new ANIM_SUB_BLOCK_HEADER_OBJ[0];
        public static Instruction_obj_header[] INSTR_OBJ_HEADER = new Instruction_obj_header[0];
        public static Instruction_Obj[] FRAME_DATA = new Instruction_Obj[0];

        public static Program prg = new Program();

        public static void Main(string[] args)
        {



            if (args != null) 
            {


                if (args.Length == 2)
                {


                    // ANIM BIN
                    //if (args[0].Substring(args[0].Length - 3, 3).ToUpper() == "BIN")
                    //{
                    
                    //    prg.Parse_ANIM(args);

                    //}


                    // run mass block extract
                    if (args[0].Substring(args[0].Length - 3, 3).ToUpper() == "BIN" && args[1] == "-e")
                    {

                        prg.EXTRACT_ANIMS(args);

                    }


                }


                if (args.Length == 1)
                {

                    if (args[0].Substring(args[0].Length - 3, 3).ToUpper() == "NBD")
                    {

                        prg.Parse_AHI(args);
                    }

                }





            }




        }




        public void Parse_ANIM(string[] args) 
        {




            Console.Write("======================================================\n");
            Console.Write("NETBIO_ANIM_PARSE Version: 0.11 \t2021, devchaps@gmail.com\n");
            Console.Write("======================================================\n\n");
            Console.Write("\t For parsing the animation .bins...\n\n");
            Console.Write("======================================================\n\n");

           

               

                    Console.WriteLine(args[0] + " " + args[1] + "\n");

                    //Analyze...
                    if (File.Exists(args[0]))
                    {

                        using (FileStream fs = new FileStream(args[0], FileMode.Open))
                        {

                            using (BinaryReader br = new BinaryReader(fs))
                            {

                                // seek to passed animation offset
                                fs.Seek(long.Parse(args[1]), SeekOrigin.Begin);
                                ANIM_BLOCK_HEADER.block_ID = br.ReadInt32();
                                ANIM_BLOCK_HEADER.sub_count = br.ReadInt32();
                                ANIM_BLOCK_HEADER.blk_sz = br.ReadInt32();



                                Console.ForegroundColor = ConsoleColor.Red;

                                Console.Write("======================================================\n");
                                Console.Write("ANIMATION BLOCK DATA                       \n");
                                Console.Write("======================================================\n\n");
                                Console.Write("ID:" + ANIM_BLOCK_HEADER.block_ID.ToString("X") + "\n");
                                Console.Write("COUNT:" + ANIM_BLOCK_HEADER.sub_count.ToString("X2") + "\n");
                                Console.Write("SIZE: " + ANIM_BLOCK_HEADER.blk_sz.ToString("") + "\n");
                                Console.Write("======================================================\n\n");


                                fs.Seek(+8, SeekOrigin.Current);

                                Array.Resize(ref ANIM_SUB_HEADER, ANIM_BLOCK_HEADER.sub_count); // RESIZE to ANIM BLOCK HEADER COUNT




                                for (int i = 0; i < ANIM_SUB_HEADER.Length; i++)
                                {
                                    ANIM_SUB_HEADER[i].subBLockID = br.ReadInt32();
                                    ANIM_SUB_HEADER[i].instruction_count = br.ReadInt32();
                                    ANIM_SUB_HEADER[i].sz = br.ReadInt32();




                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.Write("======================================================\n");
                                    Console.Write("BONE DATA " + i.ToString() + "                  \n");
                                    Console.Write("======================================================\n\n");
                                    Console.Write("ID:" + ANIM_SUB_HEADER[i].subBLockID.ToString("X") + "\n");
                                    Console.Write("COUNT:" + ANIM_SUB_HEADER[i].instruction_count.ToString("X2") + "\n");
                                    Console.Write("SIZE: " + ANIM_SUB_HEADER[i].sz.ToString("") + "\n");
                                    Console.Write("======================================================\n\n");



                                    Array.Resize(ref INSTR_OBJ_HEADER, ANIM_SUB_HEADER[i].instruction_count);

                                    for (int x = 0; x < INSTR_OBJ_HEADER.Length; x++)
                                    {
                                        string instruction_type = string.Empty;

                                        INSTR_OBJ_HEADER[x].id = br.ReadByte();
                                        INSTR_OBJ_HEADER[x].ubyte00 = br.ReadByte();
                                        INSTR_OBJ_HEADER[x].instruction_type = br.ReadByte();
                                        INSTR_OBJ_HEADER[x].t_frame = br.ReadByte();
                                        INSTR_OBJ_HEADER[x].count = br.ReadInt32();
                                        INSTR_OBJ_HEADER[x].blk_sz = br.ReadInt32();


                                if (INSTR_OBJ_HEADER[x].id == 0x01) { instruction_type = "SCALE X"; }
                                if (INSTR_OBJ_HEADER[x].id == 0x04) { instruction_type = "SCALE Z"; }
                                if (INSTR_OBJ_HEADER[x].id == 0x08) { instruction_type = "SCALE Y"; }



                                if (INSTR_OBJ_HEADER[x].id == 0x08) { instruction_type = "Rotation X"; }
                                if (INSTR_OBJ_HEADER[x].id == 0x10) { instruction_type = "Rotation Z"; }
                                if (INSTR_OBJ_HEADER[x].id == 0x20) { instruction_type = "Rotation Y"; }



                                Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.WriteLine("INSTRUCTION OBJECT DATA" + x.ToString());
                                Console.WriteLine("ID: 0x" + INSTR_OBJ_HEADER[x].id.ToString("X2") + ": " + instruction_type);
                                        Console.WriteLine("Ubyte: " + INSTR_OBJ_HEADER[x].ubyte00.ToString());
                                        Console.WriteLine("INSTRUCTION TYPE: 0x" + INSTR_OBJ_HEADER[x].instruction_type.ToString("X2"));
                                        Console.WriteLine("INSTRUCTION T_FRAME: " + INSTR_OBJ_HEADER[x].t_frame.ToString());
                                        Console.WriteLine("INSTRUCTION COUNT: " + INSTR_OBJ_HEADER[x].count.ToString());
                                        Console.WriteLine("BLOCK SIZE: " + INSTR_OBJ_HEADER[x].blk_sz.ToString() + "\n\n");


                                        Array.Resize(ref FRAME_DATA, INSTR_OBJ_HEADER[x].count); // should be as many 8 byte arrays as there are instruction counts


                                        for (int t = 0; t < FRAME_DATA.Length; t++)
                                        {


                                            // 8 bytes of instruction data per frame
                                            FRAME_DATA[t].Rotation00 = br.ReadInt16();
                                            FRAME_DATA[t].Rotation01 = br.ReadInt16();
                                            FRAME_DATA[t].Rotation02 = br.ReadInt16();
                                            FRAME_DATA[t].Rotation03 = br.ReadInt16();



                                            float total_rot00 = (float)FRAME_DATA[t].Rotation00 / 2880;
                                           float total_rot01 = (float)FRAME_DATA[t].Rotation01 / 2880;
                                           float total_rot02 = (float)FRAME_DATA[t].Rotation02 / 2880;
                                           float total_rot03 = (float)FRAME_DATA[t].Rotation02 / 2880;


                                    Console.ForegroundColor = ConsoleColor.Blue;
                                            Console.WriteLine("ROTATION " + t.ToString() + " :" + total_rot00.ToString());
                                    Console.WriteLine("ROTATION :" + t.ToString() + " :" + total_rot01.ToString());
                                    Console.WriteLine("ROTATION :" + t.ToString() + " :" + total_rot02.ToString());
                                    Console.WriteLine("ROTATION :" + t.ToString() + " :" + total_rot03.ToString());
                                    //   Console.WriteLine("ROTATION " + t.ToString() + " :" + total_rot01.ToString());
                                    //   Console.WriteLine("ROTATION :" + FRAME_DATA[t].Frame01.ToString() + "\n\n");





                                }

                                        //fs.Seek(-12, SeekOrigin.Current);
                                        //fs.Seek(+INSTR_OBJ_HEADER[x].blk_sz, SeekOrigin.Current);






                                    }







                                }





                                // RESIZE array to # of blocks in main anim block header
                                //Array.Resize(ref INSTR_HEADER, ANIM_BLOCK_HEADER.sub_count);

                                //for (int i = 0; i < INSTR_HEADER.Length; i++) 
                                //{

                                //    INSTR_HEADER[i].id = br.ReadInt32();









                            }

                        }

                    }


                
                Console.ReadLine();
            













        }



        public void EXTRACT_ANIMS(string[] args) 
        {




            Console.Write("======================================================\n");
            Console.Write("NETBIO_ANIM_PARSE Version: 0.11 \t2021, devchaps@gmail.com\n");
            Console.Write("======================================================\n\n");
            Console.Write("\t extracting animation blocks.....\n\n");
            Console.Write("======================================================\n\n");


            using (FileStream fs = new FileStream(args[0], FileMode.Open)) 
            {

                using (BinaryReader br = new BinaryReader(fs)) 
                {

                    fs.Seek(20, SeekOrigin.Begin);

                    Int32 ptr_tbl_offset = br.ReadInt32();

                    fs.Seek(ptr_tbl_offset, SeekOrigin.Begin);


                    for (int i = 0; i < 40; i++) 
                    {

                        Int32 ptr = br.ReadInt32();
                      
                         // 
                        if (ptr != -1)
                        {
                            fs.Seek(ptr + 4, SeekOrigin.Begin);
                            Int32 block_type = br.ReadInt32();

                            if (block_type == 0x0A)
                            {
                                Console.WriteLine("----------------------------------\n");

                            }

                            if (block_type != 0x500)
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.WriteLine(Group_resolve(block_type));
                            }


                            fs.Seek(ptr_tbl_offset + (4 * i), SeekOrigin.Begin);

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(ptr.ToString(""));

                        }
                        if(ptr == -1)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ptr.ToString("X2"));
                        }

                    




                    }

                
                }
            
            
            
            }





        }



        public string Group_resolve(Int32 group_val) 
        {
            switch (group_val) 
            {
                case 10: return "Lower";
                case 12: return "Upper";
                case 04: return "Hands";
                case 06: return "Face";

                default: return "Undefined";
            
            }

          
        }


        public void Parse_AHI(string[] args)
        {
            AHI_HEADER_OBJ AHI_HEADER = new AHI_HEADER_OBJ();
            AHI_BONE_OBJP[] BONE_OBJECT = new AHI_BONE_OBJP[0];

            FileStream fs = new FileStream(args[0], FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            fs.Seek(36, SeekOrigin.Begin);


            Int32 ahi_off = br.ReadInt32();

            fs.Seek(ahi_off, SeekOrigin.Begin);


            AHI_HEADER.ufloat00 = br.ReadSingle();
            AHI_HEADER.Bone_Count = br.ReadInt32();
            AHI_HEADER.t_size = br.ReadInt32();
            AHI_HEADER.uint00 = br.ReadInt32();
            AHI_HEADER.uint01 = br.ReadInt32();
            AHI_HEADER.count00 = br.ReadInt32();
            AHI_HEADER.uint02 = br.ReadInt32();


            Console.WriteLine("[HEADER]\n");
            Console.WriteLine("File size: " + AHI_HEADER.t_size.ToString());
            Console.WriteLine("Total Bone Count: " + AHI_HEADER.Bone_Count.ToString());


            Array.Resize(ref BONE_OBJECT, AHI_HEADER.Bone_Count);

            for (int i = 0; i < AHI_HEADER.Bone_Count - 1; i++)
            {

                Array.Resize(ref BONE_OBJECT[i].scale, 3);
                Array.Resize(ref BONE_OBJECT[i].QNon, 4);
                Array.Resize(ref BONE_OBJECT[i].vec3_transform, 3);

                BONE_OBJECT[i].Obj_id = br.ReadInt32();
                BONE_OBJECT[i].Uint00 = br.ReadInt32(); // always 1?
                BONE_OBJECT[i].bone_size = br.ReadInt32(); // always 268
                BONE_OBJECT[i].bone_id = br.ReadInt32();
                BONE_OBJECT[i].bone_prev = br.ReadInt32();
                BONE_OBJECT[i].bone_next = br.ReadInt32();
                BONE_OBJECT[i].bone_obj = br.ReadInt32();




                Console.WriteLine("\n--------------------BONE OBJECT---------------------\n");
                Console.WriteLine("Index: " + i.ToString());
                Console.WriteLine("Bone Sz: " + BONE_OBJECT[i].bone_size.ToString());
                Console.WriteLine("Bone ID: " + BONE_OBJECT[i].bone_id.ToString());

                if (BONE_OBJECT[i].bone_prev == -1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ROOT BONE: " + BONE_OBJECT[i].bone_prev.ToString());
                }
                else if (BONE_OBJECT[i].bone_prev != -1)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Bone Prev/Parent: " + BONE_OBJECT[i].bone_prev.ToString());
                }


                if (BONE_OBJECT[i].bone_next == -1)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Bone Next: (LEAF ELEMENT:) " + BONE_OBJECT[i].bone_next.ToString());
                }
                else if (BONE_OBJECT[i].bone_next != -1)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Bone Next/Child : " + BONE_OBJECT[i].bone_next.ToString());
                }

                Console.ForegroundColor = ConsoleColor.White;
                for (int x = 0; x < BONE_OBJECT[i].scale.Length; x++)
                {
                    BONE_OBJECT[i].scale[x] = br.ReadSingle();
                    Console.WriteLine("Scale Values: " + BONE_OBJECT[i].scale[x].ToString());
                }

                BONE_OBJECT[i].ufloat00 = br.ReadSingle();


                for (int j = 0; j < BONE_OBJECT[i].QNon.Length; j++)
                {
                    BONE_OBJECT[i].QNon[j] = br.ReadSingle();
                    Console.WriteLine("Qnon " + j.ToString() + ": " + BONE_OBJECT[i].QNon[j].ToString());

                }


                for (int y = 0; y < BONE_OBJECT[i].vec3_transform.Length; y++)
                {
                    BONE_OBJECT[i].vec3_transform[y] = br.ReadSingle();
                    Console.WriteLine("Vec3: " + y.ToString() + " " + BONE_OBJECT[i].vec3_transform[y].ToString());
                }

                fs.Seek(fs.Position + 196, SeekOrigin.Begin);
                Console.WriteLine("new bone start pos : " + fs.Position.ToString());

            }


            fs.Close();
            br.Close();

        }





    }
}

