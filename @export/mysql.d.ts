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
    * make insert into of a new record into database
    * 
    * 
     * @param table -
     * @param args -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function add(table: any, args: object, env?: object): any;
   /**
    * Create commit task data for make batch insert into current table
    * 
    * 
     * @param delayed 
     * + default value Is ``false``.
   */
   function batch_insert(table: object, delayed?: boolean): object;
   /**
    * check of the target record is existsed inside the database or not?
    * 
    * 
     * @param table -
     * @param args condition test for where closure
     * @param env 
     * + default value Is ``null``.
   */
   function check(table: object, args: object, env?: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function clear_insert_option(table: any, env?: object): object|object;
   /**
    * Close the ssh proxy connection
    * 
    * 
   */
   function close_ssh(): ;
   /**
    * commit the batch insert into database
    * 
    * 
     * @param batch -
     * @param transaction 
     * + default value Is ``null``.
   */
   function commit(batch: object, transaction?: object): ;
   /**
   */
   function count(table: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function create_filedump(dir: string, env?: object): object;
   /**
    * set delayed options for insert into
    * 
    * > this delayed options will be reste to no-delayed after insert has been called
    * 
     * @param env 
     * + default value Is ``null``.
   */
   function delayed(table: any, env?: object): object|object;
   /**
   */
   function delete(table: object): any;
   /**
   */
   function distinct(table: object): object;
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
    * exec sql and fetch result data as dataframe
    * 
    * 
     * @param table -
     * @param sql -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function exec(table: object, sql: string, env?: object): any;
   /**
     * @param args default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function find(table: object, args?: object, env?: object): any;
   /**
    * get the last mysql query that execute
    * 
    * 
     * @param mysql -
   */
   function get_last_sql(mysql: any): string;
   /**
   */
   function group_by(model: object, fields: any): object;
   /**
     * @param env default value Is ``null``.
   */
   function ignore(table: any, env?: object): object|object;
   /**
    * mysql left join
    * 
    * 
     * @param model -
     * @param table -
   */
   function left_join(model: object, table: string): object;
   /**
     * @param n default value Is ``null``.
   */
   function limit(table: object, m: object, n?: object): any;
   /**
    * on join condition test for left join operation
    * 
    * 
     * @param model -
     * @param args test condition for left join, multiple expression means AND asserts.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function on(model: object, args: object, env?: object): object;
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
     * @param ssh ssh forward configuration, is a tuple list that has data fields:
     *  
     *  1. user: ssh user name
     *  2. password: ssh password
     *  3. port: ssh server port, default is 22
     *  4. local: ssh local port for forward the connection, default is 3307
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function open(user_name?: string, password?: string, dbname?: string, host?: string, port?: object, error_log?: string, timeout?: object, connection_uri?: string, general?: boolean, ssh?: object, env?: object): object;
   /**
   */
   function open_transaction(table: object): object;
   /**
     * @param desc default value Is ``false``.
   */
   function order_by(table: object, x: any, desc?: boolean): any;
   /**
    * run the mysql performance counter in a given timespan perioid.
    * 
    * 
     * @param mysql mysql connection parameters for create a 
     *  mysql performance counter @``T:Oracle.LinuxCompatibility.LibMySQL.PerformanceCounter.Logger`` object.
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
     * @return the return tuple list data has attribute data ``global_status``, is the raw data 
     *  for the performance counter which is pulled from the mysql server.
   */
   function performance_counter(mysql: any, task: object, resolution?: number, env?: object): object;
   /**
    * make project of a single column
    * 
    * 
     * @param table -
     * @param field -
     * @param env -
     * 
     * + default value Is ``null``.
     * @return returns a element data vector
   */
   function project(table: object, field: string, env?: object): any;
   /**
    * make random sampling n rows from the given data table
    * 
    * > the random sampling of n rows is implemented based on the sql options ``order by rand() limit n``,
    * >  so do not append the order_by and limit function call to the generated table object, it will break
    * >  the sampling implementation.
    * 
     * @param table -
     * @param n -
   */
   function sampling(table: object, n: object): object;
   /**
    * make update of the database record
    * 
    * 
     * @param table -
     * @param args -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function save(table: object, args: object, env?: object): any;
   /**
    * make data pull from database
    * 
    * 
     * @param table -
     * @param args -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a dataframe object that contains the data that pull from the database
   */
   function select(table: object, args?: object, env?: object): any;
   /**
    * Create a table reference
    * 
    * 
     * @param mysql -
     * @param name -
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
