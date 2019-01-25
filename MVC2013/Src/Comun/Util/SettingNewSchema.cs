using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun.Util
{
    public class SettingNewSchema : DbMigration
    {


        public override void Up()
        {
            MoveTable(name: "dbo.Paises", newSchema: "adm");
        }

        public override void Down()
        {
           // MoveTable(name: "domain.Cars", newSchema: "dbo");
        }

    }
}