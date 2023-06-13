// export R# package module type define for javascript/typescript language
//
//    imports "mysql" from "graphR";
//
// ref=graphR.mysqlDatabase@graphR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace mysql {
   /**
     * @param env default value Is ``null``.
   */
   function create_filedump(dir: string, env?: object): object;
   /**
    * dump the inserts transaction mysql file
    * 
    * 
     * @param data A collection of the mysql row data for insert into the database
     * @param dir -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function dump_inserts(data: any, dir: string, env?: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function write_dumps(dump: object, data: any, env?: object): any;
}
