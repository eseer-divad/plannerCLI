import { Command } from '@oclif/core';
import { db } from '../../utils/db';

export default class Add extends Command {
  static description = 'Add a new task';

  async run(): Promise<void> {
    const inquirer = await require('inquirer');

    const answers = await inquirer.prompt([
      {name: 'name', message: 'Enter name (required):', type: 'input'},
      {name: 'due', message: 'Enter due date (optional):', type: 'input'},
      {name: 'priority', message: 'Enter priority (optional):', type: 'number'},
      {name: 'note', message: 'Enter note (optional):', type: 'input'},
    ]);

    const dbInstance = await db;
    const result = await dbInstance.run('INSERT INTO tasks (name, due, priority, note) VALUES (?, ?, ?, ?)', [answers.name, answers.due || null, answers.priority || null, answers.note || null]);
    this.log(`Task added with ID ${result.lastID}`);
  }
}
