using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    public static class NetvarManager
    {
        private static int SearchInSubSubTable(IntPtr subTable, string searchFor)
        {
            IntPtr subTablePointer = M.Read<IntPtr>((IntPtr)subTable + 0x28);
            IntPtr current = M.Read<IntPtr>(subTablePointer);
            while (true)
            {
                string entryName = M.ReadString(M.Read<IntPtr>((IntPtr)current));

                if (entryName == "")
                    break;

                if (entryName.Length < 1)
                    break;

                if (entryName.Length > 3)
                {
                    int offset = M.Read<int>((IntPtr)current + 0x2C);
                    if (entryName.Equals(searchFor))
                        return offset;
                }

                int subSubTable = M.Read<int>((IntPtr)current + 0x28);

                if (subSubTable > 0)
                {
                    int a = SearchInSubSubTable(current, searchFor);
                    if (a > 0)
                        return a;
                }
                current += 0x3C;
            }

            return 0;

        }
        private static int SearchInSubtable(IntPtr subTable, string searchFor)
        {
            IntPtr current = subTable;
            while (true)
            {
                string entryName = M.ReadString(M.Read<IntPtr>((IntPtr)current));

                if (entryName == "")
                    break;

                if (entryName.Length < 1)
                    break;

                if (entryName == "baseclass")
                {
                    int a = SearchInBaseClass(current, searchFor);
                    if (a > 0)
                        return a;
                }

                if (entryName == "cslocaldata")
                {
                    int a = SearchInCSLocalData(current, searchFor);
                    if (a > 0)
                        return a;
                }

                if (entryName == "localdata")
                {
                    int a = SearchInLocalData(current, searchFor);
                    if (a > 0)
                        return a;
                }

                int subSubTable = M.Read<int>((IntPtr)current + 0x28);

                if (subSubTable > 0)
                {
                    int a = SearchInSubSubTable(current, searchFor);
                    if (a > 0)
                        return a;
                }

                int offset = M.Read<int>((IntPtr)current + 0x2C);
                if (entryName == searchFor)
                    return offset;

                current += 0x3C;
            }

            return 0;
        }
        private static int SearchInBaseClass(IntPtr baseClass, string searchFor)
        {
            int a = SearchInSubtable(baseClass + 0x3C, searchFor);
            if (a > 0)
                return a;

            IntPtr baseClassPtr = M.Read<IntPtr>((IntPtr)baseClass);

            string className = M.ReadString(baseClassPtr);

            if (className.Equals("baseclass"))
                return SearchInBaseClass(M.Read<IntPtr>(M.Read<IntPtr>(baseClass + 0x28)), searchFor);

            return 0;
        }
        private static int SearchInCSLocalData(IntPtr csLocalData, string searchFor)
        {
            int a = SearchInSubtable(csLocalData + 0x28, searchFor);

            if (a > 0)
                return a;

            IntPtr csLocalDataPtr = M.Read<IntPtr>((IntPtr)csLocalData);
            IntPtr baseClassPtr = M.Read<IntPtr>((IntPtr)csLocalData + 0x28);

            string className = M.ReadString(csLocalDataPtr);

            if (className == "cslocaldata")
                return SearchInBaseClass(M.Read<IntPtr>(baseClassPtr), searchFor);

            return 0;
        }
        private static int SearchInLocalData(IntPtr localData, string searchFor)
        {
            int a = SearchInSubtable(localData + 0x28, searchFor);

            if (a > 0)
                return a;

            IntPtr localDataPtr = M.Read<IntPtr>((IntPtr)localData);
            IntPtr localDataBaseClassPtr = M.Read<IntPtr>((IntPtr)localData + 0x28);

            string className = M.ReadString(localDataPtr);

            if (className == "localdata")
                return SearchInBaseClass(M.Read<IntPtr>(localDataBaseClassPtr), searchFor);


            return 0;
        }
        private static int SearchInTableFor(IntPtr table, string searchFor)
        {
            IntPtr current = M.Read<IntPtr>(M.Read<IntPtr>(table + 0xC));
            while (true)
            {
                if (M.Read<int>(current) < 1)
                    break;

                string entryName = M.ReadString(M.Read<IntPtr>(current));

                if (entryName.Length < 1)
                    break;

                if (entryName == "baseclass")
                    return SearchInBaseClass(current, searchFor);

                if (entryName == "cslocaldata")
                    return SearchInCSLocalData(current, searchFor);

                if (entryName == "localdata")
                    return SearchInLocalData(current, searchFor);

                int offset = M.Read<int>((IntPtr)current + 0x2C);
                if (entryName.Equals(searchFor))
                    return offset;
                current += 0x3C;

            }

            return 0;
        }
        private static IntPtr GetTable(string wantedTable)
        {
            IntPtr current = Offsets.ClientClassesHead;

            while (true)
            {
                string className = M.ReadString((M.Read<IntPtr>((current + 0x8))));
                string tableName = M.ReadString(M.Read<IntPtr>(M.Read<IntPtr>(current + 0xC) + 0xC));

                if (className.Equals(wantedTable) || tableName.Equals(wantedTable))
                    return current;

                current = M.Read<IntPtr>(current + 0x10);
                if ((int)current < 1)
                    break;
            }

            return IntPtr.Zero;
        }

        public static int GetOffset(string table, string entry, int addition = 0)
        {
            IntPtr tableAddress = GetTable(table);
            int offset = SearchInTableFor(tableAddress, entry);

            H.Log($"{table}->{entry} = {offset.Hex()}", LogMode.CHILD);

            return offset + addition;
        }
    }
}
