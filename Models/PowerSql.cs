using System.Data;
using System.Net;
using System.Net.Mail;
using System.Data.SqlClient;

namespace CoreBT.Models
{
    public static class PowerSql
    {

        public static string _connectionString = "Server=.;Database=igmsys;Integrated Security=false;uid=user;pwd=user@123";

        //public static string _connectionString = @"Server=S97-74-89-136\SQLEXPRESS;Database=igmsys;Integrated Security=false;uid=sa;pwd=P@ssw0rd";

        public static object Execute(this string query)
        {
            dynamic result;
            DateTime start = DateTime.Now;
            SqlCommand sqlCommand = new SqlCommand(query);
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {
                con.Open();
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    sqlCommand.Connection = con;
                    sda.SelectCommand = sqlCommand;
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        TimeSpan ts = DateTime.Now.Subtract(start);
                        con.Close();
                        return new
                        {
                            IsError = false,
                            Query = query,
                            data = dt,
                            time = ts.TotalMilliseconds
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    return result = new
                    {
                        IsError = true,
                        data = new { },
                        Message = ex.Message
                    };
                }
                if (con.State == ConnectionState.Closed)
                    return result = new
                    {
                        IsError = true,
                        data = new { },
                        Message = "Error Occured While Connecting... "
                    };

                throw;
            }
        }
        public static object ExecuteSpecial(this string query)
        {
            dynamic result;
            DateTime start = DateTime.Now;
            SqlCommand sqlCommand = new SqlCommand(query);
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {
                con.Open();
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    sqlCommand.Connection = con;
                    sda.SelectCommand = sqlCommand;
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        TimeSpan ts = DateTime.Now.Subtract(start);
                        con.Close();
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            return new
                            {
                                IsError = false,
                                Query = query,
                                data = dt,
                                time = ts.TotalMilliseconds
                            };
                        }
                        else
                        {
                            return result = new
                            {
                                IsError = true,
                                Message = dt.Rows[0][0].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    return result = new
                    {
                        IsError = true,
                        data = new { },
                        Message = ex.Message
                    };
                }
                if (con.State == ConnectionState.Closed)
                    return result = new
                    {
                        IsError = true,
                        data = new { },
                        Message = "Error Occured While Connecting... "
                    };

                throw;
            }
        }

        public static object ExecuteProcedure(DataTable data, string para, Int64 ID)
        {
            dynamic result;
            string str = "0";
            DateTime start = DateTime.Now;
            SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
                using (SqlCommand command = new SqlCommand("SP_SAVE_FROMFIELDS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@TEMPID", SqlDbType.NVarChar).Value = para;
                    command.Parameters.Add("@data", SqlDbType.Structured).Value = data;
                    command.Parameters.Add("@FromId", SqlDbType.BigInt).Value = ID;

                    connection.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        sda.SelectCommand = command;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            TimeSpan ts = DateTime.Now.Subtract(start);
                            connection.Close();
                            return new
                            {
                                IsError = false,
                                data = dt,
                                time = ts.TotalMilliseconds
                            };
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    return result = new
                    {
                        IsError = true,
                        data = new { },
                        Message = ex.Message
                    };
                }
                if (connection.State == ConnectionState.Closed)
                    return result = new
                    {
                        IsError = true,
                        data = new { },
                        Message = "Error Occured While Connecting... "
                    };

                throw;
            }
        }


        public static object ExecuteRoleAccess(DataTable data, string Name, Int64 ID, Int64 ProjectID, string User)
        {
            dynamic result;
            string str = "0";
            DateTime start = DateTime.Now;
            SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
                using (SqlCommand command = new SqlCommand("SP_SAVE_ROLE", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Name;
                    command.Parameters.Add("@Id", SqlDbType.BigInt).Value = ID;
                    command.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                    command.Parameters.Add("@User", SqlDbType.NVarChar).Value = User;
                    command.Parameters.Add("@data", SqlDbType.Structured).Value = data;
                    command.Parameters.Add("@Message", SqlDbType.VarChar, 59);
                    command.Parameters["@Message"].Direction = ParameterDirection.Output;
                    connection.Open();
                    command.CommandTimeout = 1200;
                    command.ExecuteNonQuery();
                    str = command.Parameters["@Message"].Value.ToString();
                    if (str == "1")
                    {
                        connection.Close();
                        return result = new
                        {
                            IsError = false
                        };
                    }
                    else
                    {
                        connection.Close();
                        return result = new
                        {
                            IsError = true,
                            Message = str
                        };
                    }

                }

            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    return result = new
                    {
                        IsError = true,
                        data = new { },
                        Message = ex.Message
                    };
                }
                if (connection.State == ConnectionState.Closed)
                    return result = new
                    {
                        IsError = true,
                        data = new { },
                        Message = "Error Occured While Connecting... "
                    };

                throw;
            }
        }

        public static object CreateForm(DataTable data, string TempID, Int64 L1, Int64 L2, Int64 L3, Int64 D, string Name, string Usr, Int64 ID)
        {
            dynamic result;
            string str = "0";
            DateTime start = DateTime.Now;
            SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
                using (SqlCommand command = new SqlCommand("SP_SAVE_FROM", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@data", SqlDbType.Structured).Value = data;
                    command.Parameters.Add("@TEMPID", SqlDbType.NVarChar).Value = TempID;
                    command.Parameters.Add("@AccL1ID", SqlDbType.BigInt).Value = L1;
                    command.Parameters.Add("@AccL2ID", SqlDbType.BigInt).Value = L2;
                    command.Parameters.Add("@AccL3ID", SqlDbType.BigInt).Value = L3;
                    command.Parameters.Add("@DocID", SqlDbType.BigInt).Value = D;
                    command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Name;
                    command.Parameters.Add("@User", SqlDbType.NVarChar).Value = Usr;
                    command.Parameters.Add("@FromId", SqlDbType.BigInt).Value = ID;
                    command.Parameters.Add("@Message", SqlDbType.VarChar, 59);
                    command.Parameters["@Message"].Direction = ParameterDirection.Output;
                    connection.Open();
                    command.CommandTimeout = 1200;
                    command.ExecuteNonQuery();
                    str = command.Parameters["@Message"].Value.ToString();
                    if (str == "1")
                    {
                        connection.Close();
                        return result = new
                        {
                            IsError = false
                        };
                    }
                    else
                    {
                        connection.Close();
                        return result = new
                        {
                            IsError = true,
                            Message = str
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    return result = new
                    {
                        IsError = true,
                        Message = ex.Message
                    };
                }
                if (connection.State == ConnectionState.Closed)
                    return result = new
                    {
                        IsError = true,
                        Message = "Error Occured While Connecting... "
                    };

                throw;
            }
        }

        public static object ExecuteMultiDataSet(this string query)
        {
            dynamic result;
            DateTime start = DateTime.Now;
            SqlCommand sqlCommand = new SqlCommand(query);
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {
                con.Open();
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    sqlCommand.Connection = con;
                    sda.SelectCommand = sqlCommand;
                    using (DataSet ds = new DataSet())
                    {
                        sda.Fill(ds);
                        TimeSpan ts = DateTime.Now.Subtract(start);
                        con.Close();
                        return new
                        {
                            IsError = false,
                            Query = query,
                            data = ds,
                            time = ts.TotalMilliseconds
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    return result = new
                    {
                        IsError = true,
                        data = new { },
                        Message = ex.Message
                    };
                }
                if (con.State == ConnectionState.Closed)
                    return result = new
                    {
                        IsError = true,
                        data = new { },
                        Message = "Error Occured While Connecting... "
                    };

                throw;
            }
        }


        //public static async Task<bool> SendMailAsync(string recipient, string subject, string message)
        //{
        //    bool isSend = false;
        //    SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

        //    client.Port = 587;
        //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    client.UseDefaultCredentials = false;
        //    var credentials = new NetworkCredential
        //    {
        //        UserName = "info@igmsys.com",
        //        Password = "Igmsys@2022"
        //    };
        //    client.EnableSsl = true;
        //    client.Credentials = credentials;

        //    try
        //    {
        //        var mail = new MailMessage(_sender.Trim(), recipient.Trim());
        //        mail.Subject = subject;
        //        mail.Body = message;
        //        client.Send(mail);
        //        isSend = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return isSend;
        //    }

        //    return isSend;
        //}

        public static async Task<bool> SendEmailAsync(string email, string msg, string subject = "")
        {
            // Initialization.  
            bool isSend = false;

            try
            {
                // Initialization.  
                var body = msg;
                var message = new MailMessage();

                // Settings.  
                message.To.Add(new MailAddress(email));
                message.From = new MailAddress("info@igmsys.com");
                message.Subject = !string.IsNullOrEmpty(subject) ? subject : "Igmsys Technical Support";
                message.Body = body;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    // Settings.  
                    var credential = new NetworkCredential
                    {
                        UserName = "info@igmsys.com",
                        Password = "Igmsys@2022"
                    };

                    // Settings.  
                    smtp.Credentials = credential;
                    smtp.Host = "smtpout.secureserver.net";
                    smtp.Port = Convert.ToInt32(465);
                    smtp.EnableSsl = true;

                    // Sending  
                    await smtp.SendMailAsync(message);

                    // Settings.  
                    isSend = true;
                }
            }
            catch (Exception)
            {
                // Info  
                return isSend;
            }
            return isSend;
        }




        //// Bundle related types, functions
        //public class dynamicblockone
        //{
        //    public string CityId { get; set; }
        //    public string ShipPrice { get; set; }

        //}

        //private static DataTable GetItemsAsDataTableone(List<dynamicblockone> points)
        //{

        //    DataTable table = new DataTable();
        //    table.Columns.Add("CityId");
        //    table.Columns.Add("ShippingPrice");


        //    foreach (dynamicblockone reading in points)
        //    {
        //        table.Rows.Add(new object[] { reading.CityId, reading.ShipPrice });
        //    }

        //    return table;
        //}

        //public static object CreateBundle(string T, string B, string V, string Bt, List<dynamicblockone> dat, string Usr)
        //{
        //    string str = "0";
        //    dynamic result;
        //    SqlConnection connection = new SqlConnection(_connectionString);

        //    try
        //    {

        //        using (SqlCommand command = new SqlCommand("SP_CREATE_BUNDLE", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.Add("@TempId", SqlDbType.NVarChar, 200).Value = T;
        //            command.Parameters.Add("@BundleId", SqlDbType.BigInt).Value = Convert.ToInt64(B);
        //            command.Parameters.Add("@VersionId", SqlDbType.BigInt).Value = Convert.ToInt64(V);
        //            command.Parameters.Add("@UserId", SqlDbType.NVarChar, 128).Value = Usr;
        //            command.Parameters.Add("@Bundletype", SqlDbType.Int).Value = Convert.ToInt32(Bt);
        //            command.Parameters.Add("@data", SqlDbType.Structured).Value = GetItemsAsDataTableone(dat);
        //            command.Parameters.Add("@Message", SqlDbType.VarChar, 300);
        //            command.Parameters["@Message"].Direction = ParameterDirection.Output;
        //            connection.Open();
        //            command.CommandTimeout = 1200;
        //            command.ExecuteNonQuery();
        //            str = command.Parameters["@Message"].Value.ToString();
        //        }
        //        connection.Close();
        //        if (str == "1")
        //        {
        //            return new
        //            {
        //                IsError = false
        //            };
        //        }
        //        else
        //        {
        //            return result = new
        //            {
        //                IsError = true,
        //                Message = str
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (connection.State == ConnectionState.Open)
        //        {
        //            connection.Close();
        //            return result = new
        //            {
        //                IsError = true,
        //                Message = ex.Message
        //            };
        //        }
        //        if (connection.State == ConnectionState.Closed)
        //            return result = new
        //            {
        //                IsError = true,
        //                Message = "Error Occured While Connecting... "
        //            };

        //        throw;
        //    }
        //}


        //public static string AddProductInBundle(string tempid, string Q, string P, string I, string B, string v, DataTable data)
        //{
        //    string str = string.Empty;
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        using (SqlCommand command = new SqlCommand("SP_SAVE_PRODUCTBUNDLE_Temp", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.Add("@TempId", SqlDbType.NVarChar).Value = tempid;
        //            command.Parameters.Add("@QUANTITY", SqlDbType.NVarChar).Value = Q;
        //            command.Parameters.Add("@PRICE", SqlDbType.NVarChar).Value = P;
        //            command.Parameters.Add("@PID", SqlDbType.BigInt, 0x1388).Value = I;
        //            command.Parameters.Add("@BundleId", SqlDbType.BigInt, 0x1388).Value = B;
        //            command.Parameters.Add("@VersionId", SqlDbType.BigInt, 0x1388).Value = v;
        //            command.Parameters.Add("@data", SqlDbType.Structured).Value = data;
        //            command.Parameters.Add("@Message", SqlDbType.VarChar, 0x1388);
        //            command.Parameters["@Message"].Direction = ParameterDirection.Output;
        //            connection.Open();
        //            command.CommandTimeout = 1200;
        //            command.ExecuteNonQuery();
        //            str = command.Parameters["@Message"].Value.ToString();

        //        }
        //        connection.Close();
        //    }

        //    return str;
        //}

        //public static object ExecuteReader(this string query)
        //{

        //    SqlCommand sqlCommand = new SqlCommand(query);
        //    SqlConnection con = new SqlConnection(_connectionString);
        //    try
        //    {
        //        DateTime start = DateTime.Now;
        //        con.Open();

        //        sqlCommand.Connection = con;

        //        IDataReader rdr = new SqlCommand(query, con).ExecuteReader();

        //        dynamic results = GetDataTableFromDataReader(rdr);

        //        TimeSpan ts = DateTime.Now.Subtract(start);

        //        con.Close();

        //        return new
        //        {
        //            IsError = false,
        //            data = results,
        //            time = ts.TotalMilliseconds
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        con.Close();

        //        return new
        //        {
        //            IsError = false,
        //            data = new { },
        //            Message = ex.Message
        //        };
        //    }

        //    //  return result;
        //}

        //public static object ExecuteNonQuery(this string query)
        //{


        //    SqlConnection con = new SqlConnection(_connectionString);
        //    try
        //    {
        //        DateTime start = DateTime.Now;
        //        con.Open();
        //        SqlCommand sqlCommand = new SqlCommand(query, con);

        //        dynamic results = sqlCommand.ExecuteNonQuery();

        //        TimeSpan ts = DateTime.Now.Subtract(start);

        //        con.Close();

        //        return new
        //        {
        //            IsError = false,
        //            data = results,
        //            time = ts.TotalMilliseconds
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        con.Close();
        //        return new
        //        {
        //            IsError = true,
        //            data = new { },
        //            Message = ex.Message
        //        };
        //    }


        //}

        //public static object GetDataTableFromDataReader(IDataReader reader)
        //{

        //    DataTable schemaTable = reader.GetSchemaTable();
        //    DataTable dataTable = new DataTable();

        //    foreach (DataRow dataRow in schemaTable.Rows)
        //    {
        //        DataColumn dataColumn = new DataColumn();
        //        dataColumn.ColumnName = dataRow["ColumnName"].ToString();
        //        dataColumn.DataType = Type.GetType(dataRow["DataType"].ToString());
        //        dataColumn.ReadOnly = (bool)dataRow["IsReadOnly"];
        //        dataColumn.AutoIncrement = (bool)dataRow["IsAutoIncrement"];
        //        dataColumn.Unique = (bool)dataRow["IsUnique"];

        //        dataTable.Columns.Add(dataColumn);
        //    }


        //    while (reader.Read())
        //    {

        //        DataRow dataRow = dataTable.NewRow();

        //        for (int i = 0; i < dataTable.Columns.Count - 1; i++)
        //        {
        //            dataRow[i] = reader[i];
        //        }
        //        dataTable.Rows.Add(dataRow);

        //    }
        //    return dataTable;

        //}

        //public static bool IsNullEmptyOrWhiteSpace(this string value)
        //{

        //    if (String.IsNullOrEmpty(value) && String.IsNullOrWhiteSpace(value))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }



    public static class AlertMessages
    {
        public static object AlertMessage(bool isError, string Mesg)
        {
            object AlertMessage = new
            {
                IsError = isError,
                Title = isError ? "Action Failed" : "Action Completed",
                Message = Mesg
            };
            return AlertMessage;
        }


        public static object AlertMessage(bool isError, string Mesg, string Scope)
        {
            object AlertMessage = new
            {
                IsError = isError,
                Title = isError ? "Action Failed" : "Action Completed",
                Message = Mesg,
                Scope = Scope
            };
            return AlertMessage;
        }


        public static object AlertModal(bool isError, string Mesg)
        {
            object AlertMessage = new
            {
                IsError = isError,
                Title = "Action Failed",
                Message = Mesg
            };
            return AlertMessage;
        }



        public static object NoConfig = new
        {
            IsError = true,
            Title = "Operation Fail",
            Message = "Cannot find table configuration.Check Settings and Try Again..."
        };

        public static object InvalidConfig = new
        {
            IsError = true,
            Title = "Operation Fail",
            Message = "Invalid table configuration.Check Settings and Try Again..."
        };

        public static object ModelError = new
        {
            IsError = true,
            Title = "Operation Fail",
            Message = "Validate your inputs & try again"
        };

        public static object DBError = new
        {
            IsError = true,
            Title = "Operation Failed",
            Message = "An Error Occured While Connecting to DataBase.",
        };

        public static object SuccessResponse = new
        {
            IsError = false,
            Title = "Operation Success",
            Message = "Operation successfully completed"
        };

        public static object FailureResponse = new
        {
            IsError = true,
            Title = "Operation Fail",
            Message = "No record affected"
        };

        public static object RangeConflict = new
        {
            IsError = true,
            Title = "Operation Fail",
            Message = "Range conflict with existing values"
        };

        public static object InvalidRequest = new
        {
            IsError = true,
            Title = "Operation Fail",
            Message = "Not a Valid Request"
        };

    }

    public static class SiteSettings
    {
        public const string GoogleRecaptchaSecretKey = "6LfuoLkaAAAAAH-Qd3jP3wJr4H1L9YefGJAoWs1X";
        public const string GoogleRecaptchaSiteKey = "6LfuoLkaAAAAANDQVeBENLzuR0fP75JcLxi-eTr1";
    }

}
