use clap::{command, ArgMatches, Arg};

fn main() {
    // Use the `command!()` macro and `Arg::new` with the argument name as a str literal
    // chains the clap methods & arguments together
    let match_result: ArgMatches = command!()
        .arg(Arg::new("firstname"))
        .arg(Arg::new("lastname"))
        .get_matches();

    // utilize the arg matches by printing them
    if let Some(firstname) = match_result.value_of("firstname") {
        println!("First name: {}", firstname);
    }
    if let Some(lastname) = match_result.value_of("lastname") {
        println!("Last name: {}", lastname);
    }
}
