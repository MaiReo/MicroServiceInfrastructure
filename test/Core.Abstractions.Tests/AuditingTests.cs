using Core.TestBase;
using Shouldly;
using System.Linq;
using Xunit;

namespace Core.Abstractions.Tests
{
    public class AuditingTests : AbstractionTestBase<AuditingTests, TestDbContext>
    {
        [Fact(DisplayName = "当前用户")]
        public void CurrentUserTest()
        {
            Session.User.Id.ShouldBe(TestConsts.CURRENT_USER_ID);
            Session.User.Name.ShouldBe(TestConsts.CURRENT_USER_NAME);

            LoginAs("otherUserId", "otherUserName");

            Session.User.Id.ShouldBe("otherUserId");
            Session.User.Name.ShouldBe("otherUserName");
        }

        [Fact(DisplayName = "自动设置当前用户")]
        public void AutoSetCurrentUserTest()
        {
            LoginAs("addUserId", "addUserName");
            UsingDbContext(context => context.TestEntityFullAuditeds.Add(new TestEntityFullAudited
            {
                Name = "testname"
            }));
            UsingDbContext(context =>
            {
                var entity = context.TestEntityFullAuditeds.FirstOrDefault(x => x.Name == "testname");
                entity.ShouldNotBeNull();
                entity.CreationUserId.ShouldBe("addUserId");
                entity.CreationUserName.ShouldBe("addUserName");
            });
            LoginAs("modUserId", "modUserName");
            UsingDbContext(context =>
            {
                var entity = context.TestEntityFullAuditeds.FirstOrDefault(x => x.Name == "testname");
                entity.ShouldNotBeNull();
                entity.Age = 1;
            });
            UsingDbContext(context =>
            {
                var entity = context.TestEntityFullAuditeds.FirstOrDefault(x => x.Name == "testname");
                entity.ShouldNotBeNull();
                entity.Age.ShouldBe(1);
                entity.CreationUserId.ShouldBe("addUserId");
                entity.CreationUserName.ShouldBe("addUserName");
                entity.LastModificationTime.ShouldNotBeNull();
                entity.LastModifierUserId.ShouldBe("modUserId");
                entity.LastModifierUserName.ShouldBe("modUserName");
            });
            LoginAs("delUserId", "delUserName");
            UsingDbContext(context =>
            {
                var entity = context.TestEntityFullAuditeds.FirstOrDefault(x => x.Name == "testname");
                entity.ShouldNotBeNull();
                context.TestEntityFullAuditeds.Remove(entity);
                context.SaveChanges();
                entity.CreationUserId.ShouldBe("addUserId");
                entity.CreationUserName.ShouldBe("addUserName");
                entity.LastModificationTime.ShouldNotBeNull();
                entity.LastModifierUserId.ShouldBe("modUserId");
                entity.LastModifierUserName.ShouldBe("modUserName");
                entity.DeletionTime.ShouldNotBeNull();
                entity.DeleterUserId.ShouldBe("delUserId");
                entity.DeleterUserName.ShouldBe("delUserName");
            });
        }
    }
}
