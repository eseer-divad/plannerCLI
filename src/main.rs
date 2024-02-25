use clap::{command, Arg};

fn main() {
    println!("Hello, plannerCLI user!");
    command!().arg(
        Arg::new("firstname")
    );
}
