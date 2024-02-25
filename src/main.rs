use clap::{command, ArgMatches, Arg};

fn main() {
    let match_result: ArgMatches = command!()
        .arg(Arg::new("firstname"))
        .arg(Arg::new("lastname"))
        .get_matches();

    if let Some(firstname) = match_result.value_of("firstname") {
        println!("First name: {}", firstname);
    }
    if let Some(lastname) = match_result.value_of("lastname") {
        println!("Last name: {}", lastname);
    }
}
