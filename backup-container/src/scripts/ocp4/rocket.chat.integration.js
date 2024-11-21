class Script {
  /**
   * @params {object} request
   */
  process_incoming_request({ request }) {
    let data = request.content;
    let attachmentColor = `#36A64F`;
    let statusMsg = `Successful backup`; // Default message

    // Check for "Backup Size: 0" in the message
    const backupSizeZero = /Backup Size: 0/.test(data.message);

    // Determine if there's an error
    let isError = data.statusCode === `ERROR` || backupSizeZero;

    if (isError) {
      statusMsg = `Failed backup`; // Change message for errors
      attachmentColor = `#A63636`; // Change color to red

      // Replace "Successfully backed up" in data.message with "Backup failed"
      data.message = data.message.replace('Successfully backed up', 'Backup failed');
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

    // Add bold formatting for error messages
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
