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
}
