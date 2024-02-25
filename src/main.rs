mod db;
use clap::{command, Arg};
use db::{create_table, insert_tasks, query_tasks, open_db};
use rusqlite::Result;

fn main() -> Result<()> {
    let matches = command!()
        .name("Task Manager")
        .arg(Arg::new("add")
            .long("add")
            .help("Adds a new task. Format: name|time_due|priority|note")
            .takes_value(true))
        .arg(Arg::new("list")
            .long("list")
            .help("Lists all tasks")
            .takes_value(false))
        .get_matches();

    let conn = open_db("tasks.db")?;
    create_table(&conn)?;

    if let Some(details) = matches.value_of("add") {
        let parts: Vec<&str> = details.split('|').collect();
        if parts.len() < 3 {
            println!("Error: You must specify the task details in the format: name|time_due|priority|note");
            return Ok(());
        }

        let name = parts[0];
        let time_due = parts.get(1).unwrap_or(&"No Due Time");

        // Explicitly handle the parsing of priority and convert any error to rusqlite::Error
        let priority_result = parts.get(2).unwrap_or(&"1").parse::<i32>();
        let priority = match priority_result {
            Ok(p) => p,
            Err(_) => return Err(rusqlite::Error::ExecuteReturnedResults), // Use an appropriate error
        };

        let note = parts.get(3).unwrap_or(&"");

        insert_tasks(&conn, name, time_due, priority, note)?;
        println!("Task added successfully.");
    } else if matches.is_present("list") {
        query_tasks(&conn)?;
    }

    Ok(())
}
