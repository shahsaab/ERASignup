using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


/// <summary>
/// Summary description for DAL
/// </summar>
public class DAL
{
    public SqlConnection con;
    SqlCommand cmd;
    SqlDataAdapter ada;
    DataTable dt;
    public string ExceptionMsg = null;
    public DAL(string conName = null)
    {
        if (conName == null)
            con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["CS"].ConnectionString);
        else
            con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["CS2"].ConnectionString);

    }

    public DataTable execQuery(string Query, CommandType ct, SqlParameter[] param)
    {
        try
        {
            cmd = new SqlCommand(Query, con);
            cmd.CommandTimeout = 60; //Seconds
            if (ct == CommandType.StoredProcedure)
            {
                cmd.CommandType = ct;
                cmd.Parameters.AddRange(param);
            }
            ada = new SqlDataAdapter(cmd);
            dt = new DataTable();
            ada.Fill(dt);

            return dt;
        }
        catch (Exception ex)
        {
            ExceptionMsg = ex.Message;
            return null;
        }
    }

}