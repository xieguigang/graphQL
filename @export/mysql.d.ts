// export R# package module type define for javascript/typescript language
//
//    imports "mysql" from "graphR";
//
// ref=graphR.mysqlDatabaseTool@graphR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

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
     * @param n default value Is ``null``.
   */
   function limit(table: object, m: object, n?: object): any;
   /**
    * open a mysql connection, construct a database model
    * 
    * 
     * @param user_name -
     * 
     * + default value Is ``null``.
     * @param password -
     * 
     * + default value Is ``null``.
     * @param dbname -
     * 
     * + default value Is ``null``.
     * @param host -
     * 
     * + default value Is ``'localhost'``.
     * @param port -
     * 
     * + default value Is ``3306``.
     * @param error_log 
     * + default value Is ``null``.
     * @param timeout 
     * + default value Is ``-1``.
     * @param connection_uri 
     * + default value Is ``null``.
     * @param general 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function open(user_name?: string, password?: string, dbname?: string, host?: string, port?: object, error_log?: string, timeout?: object, connection_uri?: string, general?: boolean, env?: object): object;
   /**
    * run the mysql performance counter in a given timespan perioid.
    * 
    * 
     * @param mysql -
     * @param task the timespan value for run current performance counter task, value could be generates 
     *  from the time related R# base function: 
     *  
     *  ``hours``, ``minutes``, ``seconds``, ``days``, ``time_span``.
     * @param resolution the mysql performance counter data sampling time resolution value, 
     *  time internal data unit in seconds.
     * 
     * + default value Is ``1``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function performance_counter(mysql: any, task: object, resolution?: number, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function project(table: object, field: string, env?: object): any;
   /**
     * @param args default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function select(table: object, args?: object, env?: object): any;
   /**
   */
   function table(mysql: object, name: string): object;
   /**
     * @param env default value Is ``null``.
   */
   function where(table: object, args: object, env?: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function write_dumps(dump: object, data: any, env?: object): any;
}
