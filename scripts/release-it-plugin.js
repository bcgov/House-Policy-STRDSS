import path from 'path';
import { fileURLToPath } from 'url';
import { exec } from 'child_process';
import { Plugin } from 'release-it';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const frontendDir = path.join(__dirname, '../', 'frontend');

export default class MyPlugin extends Plugin {
  bump(version) {
    exec(`cd ${frontendDir}; npm version ${version} --no-git-tag-version`);
  }
}
