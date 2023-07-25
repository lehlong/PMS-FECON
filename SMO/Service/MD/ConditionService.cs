﻿using NHibernate;
using SharpSapRfc;
using SharpSapRfc.Plain;
using SMO.Core.Entities;
using SMO.Repository.Implement.MD;
using SMO.SAPINT;
using SMO.SAPINT.Function;
using SMO.Service.AD;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SMO.Service.MD
{
    public class ConditionService : GenericService<T_MD_CONDITION, ConditionRepo>
    {
        public ConditionService() : base()
        {

        }

        public void Synchronize()
        {
            try
            {
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();
                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                    systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var result = conn.ExecuteFunction(new SynMD_Condition_Function()).ToList();

                    SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString);
                    objConn.Open();

                    string tableName = "T_MD_CONDITION";
                    SqlDataAdapter daAdapter = new SqlDataAdapter("SELECT * FROM " + tableName, objConn);
                    daAdapter.UpdateBatchSize = 0;
                    DataSet dataSet = new DataSet(tableName);
                    daAdapter.FillSchema(dataSet, SchemaType.Source, tableName);
                    daAdapter.Fill(dataSet, tableName);
                    DataTable dataTable;
                    dataTable = dataSet.Tables[tableName];

                    var actionBy = ProfileUtilities.User.USER_NAME;
                    var actionDate = DateTime.Now;
                    foreach (var item in result)
                    {
                        var row = dataTable.Rows.Find(item.CODE);
                        if (row == null)
                        {
                            DataRow newRow = dataTable.NewRow();
                            newRow["CODE"] = item.CODE;
                            newRow["NAME"] = item.NAME;
                            newRow["TYPE"] = item.TYPE;
                            newRow["ACTIVE"] = "N";
                            newRow["CREATE_BY"] = actionBy;
                            newRow["CREATE_DATE"] = actionDate;
                            dataTable.Rows.Add(newRow);
                        }
                        else if (row["NAME"].ToString() != item.NAME || row["TYPE"].ToString() != item.TYPE)
                        {
                            row.BeginEdit();
                            row["NAME"] = item.NAME;
                            row["TYPE"] = item.TYPE;
                            row["UPDATE_BY"] = actionBy;
                            row["UPDATE_DATE"] = actionDate;
                            row.EndEdit();
                        }
                    }

                    SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(daAdapter);
                    daAdapter.Update(dataSet, tableName);
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }
    }
}
