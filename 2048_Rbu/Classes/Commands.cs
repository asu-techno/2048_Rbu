using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using AS_Library.Events;
using AS_Library.Graphics;
using AS_Library.Link;
using AS_Library;
using AS_Library.Readers;
using AsLibraryCore.Events.Classes;
using _2048_Rbu.Classes;
using Npgsql;
using ServiceLibCore.Classes;
using AS_Library.Classes;
using AS_Library.Events.Classes;
using Tag = AS_Library.Classes.Tag;

namespace _2048_Rbu.Classes
{
    /// <summary>
    /// Description of Commands.
    /// </summary>
    public static class Commands
    {
        public enum Types
        {
            Int16,
            Int32,
            UInt16,
            UInt32,
            Real,
            String,
            Bool
        }

        public static void Archive_OnClick(OpcServer.OpcList opcName, string nameStation = "")
        {
            try
            {
                bool postgresql = ServiceData.GetInstance().GetSqlName() == "PostgreSQL";
                WindowArchive window = new WindowArchive(OpcServer.GetInstance().GetConnectionStringData(opcName), null,
                    0, true, OpcServer.GetInstance().GetOpc(opcName).AnalogTags, OpcServer.GetInstance().GetOpc(opcName).DiscreteTags,
                    postgresql, OpcServer.GetInstance().GetObjectData(opcName).SqlTableName, nameStation, new DataNewArchiverReader());
                window.SaveGraphLeg += OnSaveGraphLeg;
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex.Message);
            }
        }

        private static void OnSaveGraphLeg(Tag tag, string nameBase)
        {
            bool postgresql = ServiceData.GetInstance().GetSqlName() == "PostgreSQL";
            if (!postgresql)
            {
                using (SqlConnection connection = new SqlConnection(ServiceData.GetInstance().GetOpcTablesBase()))
                {
                    try
                    {
                        connection.Open();

                        // Create an instance of a DataAdapter.
                        SqlDataAdapter adapter
                            = new SqlDataAdapter(
                                "SELECT Id, Legend, Koef, Color, ChangeVal, SaveByTime, RarelyChanging FROM " + nameBase + " WHERE NumTag = " + tag.NumTag,
                                connection);

                        // Create an instance of a DataSet, and retrieve data from the Authors table.
                        DataSet dbOpcTables = new DataSet("DbOpcTables");
                        adapter.FillSchema(dbOpcTables, SchemaType.Source, nameBase);
                        adapter.Fill(dbOpcTables, nameBase);

                        DataColumn[] myKey = new DataColumn[1];
                        myKey[0] = dbOpcTables.Tables[0].Columns[0];
                        dbOpcTables.Tables[0].PrimaryKey = myKey;

                        var tblAuthors = dbOpcTables.Tables[nameBase];
                        var dataRow = tblAuthors.Rows[0];
                        dataRow.BeginEdit();
                        dataRow["Legend"] = tag.NameTag;
                        dataRow["Color"] = tag.Color;
                        dataRow["Koef"] = Convert.ToDouble(tag.Koef);
                        dataRow["ChangeVal"] = Convert.ToDouble(tag.ChangeVal);
                        dataRow["SaveByTime"] = tag.SaveByTime;
                        dataRow["RarelyChanging"] = tag.RarelyChanging;
                        dataRow.EndEdit();

                        SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(adapter);
                        adapter.Update(dbOpcTables, nameBase);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Что-то пошло не так!" + "\n" + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(ServiceData.GetInstance().GetOpcTablesBase()))
                {
                    try
                    {
                        connection.Open();

                        // Create an instance of a DataAdapter.
                        NpgsqlDataAdapter adapter
                            = new NpgsqlDataAdapter(
                                "SELECT \"Id\", \"Legend\", \"Koef\", \"Color\", \"ChangeVal\", \"SaveByTime\", \"RarelyChanging\" FROM dbo." + "\"" + nameBase + "\"" + " WHERE \"NumTag\" = " + tag.NumTag,
                                connection);

                        // Create an instance of a DataSet, and retrieve data from the Authors table.
                        DataSet dbOpcTables = new DataSet("DbOpcTables");
                        adapter.FillSchema(dbOpcTables, SchemaType.Source, nameBase);
                        adapter.Fill(dbOpcTables, nameBase);

                        DataColumn[] myKey = new DataColumn[1];
                        myKey[0] = dbOpcTables.Tables[0].Columns[0];
                        dbOpcTables.Tables[0].PrimaryKey = myKey;

                        var tblAuthors = dbOpcTables.Tables[nameBase];
                        var dataRow = tblAuthors.Rows[0];
                        dataRow.BeginEdit();
                        dataRow["Legend"] = tag.NameTag;
                        dataRow["Color"] = tag.Color;
                        dataRow["Koef"] = Convert.ToDouble(tag.Koef);
                        dataRow["ChangeVal"] = Convert.ToDouble(tag.ChangeVal);
                        dataRow["SaveByTime"] = tag.SaveByTime;
                        dataRow["RarelyChanging"] = tag.RarelyChanging;
                        dataRow.EndEdit();

                        NpgsqlCommandBuilder objCommandBuilder = new NpgsqlCommandBuilder(adapter);
                        adapter.Update(dbOpcTables, nameBase);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Что-то пошло не так!" + "\n" + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            foreach (var item in OpcServer.GetInstance().GetObjects())
            {
                if (item.Value.SqlTableName == nameBase)
                {
                    var tagD = OpcServer.GetInstance().GetOpc(item.Key).DiscreteTags
                        .SingleOrDefault(x => x.NumTag == tag.NumTag);
                    if (tagD != null)
                    {
                        tagD.Legend = tag.NameTag;
                        tagD.Color = tag.Color;
                        tagD.Koef = tag.Koef;
                        tagD.ChangeVal = tag.ChangeVal;
                        tagD.SaveByTime = tag.SaveByTime;
                        tagD.RarelyChanging = tag.RarelyChanging;
                        break;
                    }

                    var tagA = OpcServer.GetInstance().GetOpc(item.Key).AnalogTags
                        .SingleOrDefault(x => x.NumTag == tag.NumTag);
                    if (tagA != null)
                    {
                        tagA.Legend = tag.NameTag;
                        tagA.Color = tag.Color;
                        tagA.Koef = tag.Koef;
                        tagA.ChangeVal = tag.ChangeVal;
                        tagA.SaveByTime = tag.SaveByTime;
                        tagA.RarelyChanging = tag.RarelyChanging;
                        break;
                    }
                }
            }
        }

        public static void EventsArchive_OnClick(OpcServer.OpcList opcName)
        {
            try
            {
                bool postgresql = ServiceData.GetInstance().GetSqlName() == "PostgreSQL";
                WindowEventsEntity window = new WindowEventsEntity(EventsBase.GetInstance().GetControlEvents(opcName), AsLibraryCore.LibService.GetInstance().GetEventsDbConnectionString(), 0, postgresql);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex.Message);
            }
        }
    }
}
