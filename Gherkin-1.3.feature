Scenario: Unable to upload new image due to missing description
Given browser "Google Chrome"
When I open "{environment.URL}"
Then I should see text “Required” in “File description error placeholder”

Scenario: Unable to upload new image due to image size greater than 500 KB
Given browser "Google Chrome"
When I open "{environment.URL}"
And I set "file description" value to "File description placeholder"
And I click on "Choose File button"
And I click on "image.jpg" greater than 500 KB in size
And I press on “ENTER”
And I click on "Submit image button"
Then I should see text “File size greater than 500 KB” in “Choose file error placeholder”

Scenario: Upload new image without success due to expired AWS token
Given browser "Google Chrome"
Given AWS token in credential file stored in system has expired
When I open "{environment.URL}"
And I set "file description" value to "File description placeholder"
And I click on "Choose File button"
And I click on "image.jpg" less than or equal in size to 500 KB
And I press on “ENTER”
And I click on "Submit image button"
Then I should see text “Failed” in “Response message placeholder”
And I should see text “Required” in “File description error placeholder”

Scenario: Upload new image with success
Given browser "Google Chrome"
When I open "{environment.URL}"
And I set "file description" value to "File description placeholder"
And I click on "Choose File button"
And I click on "image.jpg" less than or equal in size to 500 KB
And I press on “ENTER”
And I click on "Submit image button"
Then I should see text “Success” in “Response message placeholder”