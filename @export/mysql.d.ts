// export R# package module type define for javascript/typescript language
//
//    imports "mysql" from "graphR";
//
// ref=graphR.mysqlDatabase@graphR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * MySQL (/ˌmaɪˌɛsˌkjuːˈɛl/) is an open-source relational database management system (RDBMS).
 *  Its name is a combination of "My", the name of co-founder Michael Widenius's daughter My,
 *  and "SQL", the acronym for Structured Query Language. A relational database organizes data 
 *  into one or more data tables in which data may be related to each other; these relations 
 *  help structure the data. SQL is a language that programmers use to create, modify and extract
 *  data from the relational database, as well as control user access to the database. In 
 *  addition to relational databases and SQL, an RDBMS like MySQL works with an operating system 
 *  to implement a relational database in a computer's storage system, manages users, allows 
 *  for network access and facilitates testing database integrity and creation of backups.
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
    * open a mysql connection, construct a database model
    * 
    * 
     * @param user_name -
     * @param password -
     * @param dbname -
     * @param host -
     * 
     * + default value Is ``'localhost'``.
     * @param port -
     * 
     * + default value Is ``3306``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function open(user_name: string, password: string, dbname: string, host?: string, port?: object, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function write_dumps(dump: object, data: any, env?: object): any;
}
