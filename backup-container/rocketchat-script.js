/**
 * Place this script into rocketchat Incoming WebHook Integration.
 * Source: https://github.com/BCDevOps/backup-container/blob/master/scripts/ocp4/rocket.chat.integration.js
 */

class Script {
  /**
   * @params {object} request
   */
  process_incoming_request({ request }) {
    let data = request.content;
    let attachmentColor = `#36A64F`;
    let statusMsg = `Status`;
    let isError = data.statusCode === `ERROR`;
    if (isError) {
      statusMsg = `Error`;
      attachmentColor = `#A63636`;
    }

    let friendlyProjectName = ``;
    if (data.projectFriendlyName) {
      friendlyProjectName = data.projectFriendlyName;
    }

    let projectName = ``;
    if (data.projectName) {
      projectName = data.projectName;
      if (!friendlyProjectName) {
        friendlyProjectName = projectName;
      }
    }

    if (projectName) {
      statusMsg += ` message received from [${friendlyProjectName}](https://console.apps.emerald.devops.gov.bc.ca/k8s/cluster/projects/b0471a-prod/workloads):`;
    } else {
      statusMsg += ` message received:`;
    }

    if (isError) {
      statusMsg = `**${statusMsg}**`;
    }

    return {
      content: {
        text: statusMsg,
        attachments: [
          {
            text: `${data.message}`,
            color: attachmentColor,
          },
        ],
      },
    };
  }
}
