using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums.Money;
using BusinessLogic.Logger;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery {
    /// <summary>
    /// Отвечает за создание/получение пользователя
    /// </summary>
    public class UsersQuery : BaseQuery, IUsersQuery, ICleaner {
        #region IUsersQuery Members

        public Action<long> OnChangeLastActivity { get; set; }

        public long GetByHash(string userHash, string ip) {
            if (string.IsNullOrEmpty(userHash)) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "UsersQuery.GetByHash передан пустой уникальный хэш. IP-адрес пользователя = {0}", ip);
                return IdValidator.INVALID_ID;
            }

            User foundUser = Adapter.ReadByContext(c => {
                User user = c.User.FirstOrDefault(e => e.UniqueHash == userHash);
                if (user != null && user.LastActivity < DateTime.Today) {
                    //обновить данные пользователя
                    user.LastActivity = DateTime.Now;
                    user.LastIp = ip;
                    c.SaveChanges();

                    AfterChangeActivity(user.Id);
                }
                return user;
            });
            if (foundUser == null) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "UsersQuery.GetByHash не удалось найти пользователя с уникальным хэшом {0}. IP-адрес пользователя = {1}",
                    userHash, ip);
                return IdValidator.INVALID_ID;
            }
            return foundUser.Id;
        }

        public long CreateByHash(string userHash, string ip) {
            if (string.IsNullOrEmpty(userHash)) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "UsersQuery.CreateByHash передан пустой уникальный хэш. IP-адрес пользователя = {0}", ip);
                return IdValidator.INVALID_ID;
            }
            long result = IdValidator.INVALID_ID;
            Adapter.ActionByContext(c => {
                DateTime now = DateTime.Now;
                var user = new User {
                    UniqueHash = userHash,
                    CreationDate = now,
                    LastActivity = now,
                    CreationIp = ip,
                    LastIp = ip
                };
                c.User.Add(user);
                c.SaveChanges();
                if (IdValidator.IsValid(user.Id)) {
                    result = user.Id;
                } else {
                    result = IdValidator.INVALID_ID;
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "UsersQuery.CreateByHash не удалось создать пользователя с уникальным хэшом {0}. IP-адрес пользователя = {1}",
                        userHash, ip);
                }
            });
            return result;
        }

        public bool RemoveByLastActivity(DateTime maxLastActivity) {
            return Adapter.ReadByContext(c => {
                const string SQL_COMMAND = "delete from [User] where LastActivity<{0}";
                int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                         new object[] {
                                                             maxLastActivity
                                                         });
                return true;
            });
        }

        public List<long> GetAllUserIds() {
            return Adapter.ReadByContext(c => c.User.Select(e => e.Id).ToList(), new List<long>(0));
        }

        public bool UpdateEmail(long id, string email) {
            if (string.IsNullOrEmpty(email) || email.Length > User.EMAIL_LENGTH) {
                return false;
            }
            //TODO: написать нормальную поддержку обновлений
            return Adapter.ReadByContext(c => {
                const string SQL_COMMAND = "update [User] set [Email]={0} where id={1}";
                int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                         new object[] {
                                                             email,
                                                             id
                                                         });
                return true;
            });
        }

        #endregion

        private void AfterChangeActivity(long userId) {
            if (OnChangeLastActivity == null) {
                return;
            }

            OnChangeLastActivity(userId);
        }

        public bool Clean(DateTime maxDateForRemove) {
            var res = Adapter.ActionByContext(c => {
                                                  const string SQL_COMMAND = "delete FROM [User]"
                                                    + " from [User] u left join [Payment] p "
                                                    + " on u.ID = p.UserID left join [PurchasedGoods] pg on u.ID = pg.UserID"
                                                    + " where LastActivity <= {0} and (p.id is null or p.[Status] != {1})"
                                                    + " and (pg.id is null or (pg.[Status] not in ({2}, {3})))";
                                                  c.Database.ExecuteSqlCommand(SQL_COMMAND, maxDateForRemove, 
                                                      PaymentStatus.Success, PurchasedStatus.Success, PurchasedStatus.PostToCustomer);
                                              });
            return res;
        }
    }
}