# DECISIONS

Answer **yourself**, in your own words, in 3–5 sentences per question.
Refer to concrete files and classes from your own repo (for example `Features/Bookings/CreateBooking/...`).
An unanswered question gives 0 points.

## 1. A rule you moved
Pick one business rule (R1–R8). In which file did it live in the starter and in which file does it live now? Why there?

La început se face în BookingService.Create, din folderul Services. Acum am mutat regula in domeniu, mai exact în metoda Booking.Overlaps din fișierul Domain/Bookings/Booking.cs. Tot acolo am pus si anularea rezervarii în metoda Booking.Cancel.


## 2. Result instead of an exception
Why is `Result` a better choice than an exception for "the room is occupied"? What happens in your code when an `Error` is returned?

Pentru situația în care camera este ocupată, o exceptie ar fi folosită doar pentru a semnala o eroare, chiar daca aceasta este o situație prevăzută de aplicație. Eu am folosit Result<Booking> și Error.Conflict.

În momentul în care handler-ul primește o eroare, nu oprește aplicația cu o exceptie. El returnează eroarea mai departe, iar ResultHttpExtensions o transforma într-un răspuns HTTP 409 și un ProblemDetails.

## 3. Who knows about EF Core?
Which classes in your project know about EF Core and which do not? What do you gain and what do you pay with Repository + Unit of Work?

_(your answer here)_

## 4. The flow of a request
Draw in text (ASCII arrows) the path of a `POST /api/rooms/{roomId}/bookings` request through your project, with the **real** names of files and classes, from the endpoint to the database.

_(your answer here)_

## 5. What you rejected from what the AI gave you
Give a concrete example of code or an idea proposed by the AI that you corrected or rejected, and say why.

_(your answer here)_

## 6. If you had 30 more minutes
What would you improve and what risk did you leave in the project?

_(your answer here)_
