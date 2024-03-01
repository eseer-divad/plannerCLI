mod db;
use clap::{command, Arg, Command};
use db::{create_table, insert_tasks, query_tasks, open_db};
use rusqlite::Result;


// command matches
fn main() -> Result<(), rusqlite::Error> {
    let matches = command!()
        .name("Task Manager")
        .subcommand(
            Command::new("add")
                .about("Adds a new task")
                .arg(Arg::new("name")
                    .long("name")
                    .help("Task Specifier / Main Title")
                    .takes_value(true)
                )
                .arg(Arg::new("due")
                    .long("time_due")
                    .help("Defines the due date for this task.")
                    .takes_value(true)
                )
                .arg(Arg::new("pr")
                    .long("priority")
                    .help("Defines the priority for this task.")
                    .takes_value(true)
                )
                .arg(Arg::new("note")
                    .long("note")
                    .help("Adds a note aside from the main title")
                    .takes_value(true)
                )
        )
        .arg(Arg::new("list")
            .long("list")
            .help("Lists all tasks")
            .takes_value(false))
        .get_matches();

        // delete a task

    let conn = open_db("tasks.db")?;
    create_table(&conn)?;
    
    // handles how arguments for "add" work together
    if let Some(add_matches) = matches.subcommand_matches("add") {
        let name = add_matches.value_of("name").unwrap_or("ERROR");
        let due = add_matches.value_of("due").unwrap_or("N/A");
        let pr_str = add_matches.value_of("pr").unwrap_or("N/A");
        let pr: i32 = pr_str.parse().unwrap_or(-1);
        let note = add_matches.value_of("note").unwrap_or("N/A");
        
        // call insertion method in db.rs
        insert_tasks(&conn, name, due, pr, note)?;
        println!("Task added.");
    }
    else if matches.is_present("list") {
        query_tasks(&conn)?;
    }
    else {
        println!("Program runs. No recognizeable flags / arguments given.")
    }
    Ok(())
}

