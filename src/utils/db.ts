// src/utils/database.ts
import * as sqlite3 from 'sqlite3';
import { open, Database } from 'sqlite';

sqlite3.verbose();

async function initDb() {
  const db = await open({
    filename: './data.db',
    driver: sqlite3.Database,
  });

  await db.exec(`CREATE TABLE IF NOT EXISTS tasks (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    due DATE,
    priority INTEGER,
    note TEXT,
    added DATE DEFAULT CURRENT_DATE
  )`);

  return db;
}

export const db = initDb();