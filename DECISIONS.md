# DECISIONS

Answer **yourself**, in your own words, in 3–5 sentences per question.
Refer to concrete files and classes from your own repo (for example `Features/Bookings/CreateBooking/...`).
An unanswered question gives 0 points.

## 1. A rule you moved
Pick one business rule (R1–R8). In which file did it live in the starter and in which file does it live now? Why there?

La început se face în BookingService.Create, din folderul Services. Acum am mutat regula in domeniu, mai exact în metoda Booking.Overlaps din fișierul Domain/Bookings/Booking.cs. Tot acolo am pus si anularea rezervarii în metoda Booking.Cancel.


## 2. Result instead of an exception
Why is `Result` a better choice than an exception for "the room is occupied"? What happens in your code when an `Error` is returned?

Daca o camera este deja ocupata, eu am ales sa nu folosesc o exceptie pentru aceasta situatie, chiar daca este una prevazuta in aplicatie. In schimb, am folosit Result<Booking> si Error.Conflict.

## 3. Who knows about EF Core?
Which classes in your project know about EF Core and which do not? What do you gain and what do you pay with Repository + Unit of Work?

Doar clasele din Infrastructure/Persistence folosesc EF Core. Acestea sunt RoomHubDbContext, RoomRepository și BookingRepository. Clasa Room, clasa Booking, handler-ele si endpointurile nu comunica direct cu baza de date.

## 4. The flow of a request
Draw in text (ASCII arrows) the path of a `POST /api/rooms/{roomId}/bookings` request through your project, with the **real** names of files and classes, from the endpoint to the database.

Am renuntat la ideea de a folosi proprietati publice cu set. In varianta initiala, clasele aveau settere publice, ceea ce insemna ca puteau fi modificate destul de usor din orice parte a codului.

## 5. What you rejected from what the AI gave you
Give a concrete example of code or an idea proposed by the AI that you corrected or rejected, and say why.

As mai adauga teste atat pentru domeniu, cat si pentru API. As verifica mai multe situatii, cum ar fi rezervarile care se termina exact cand incepe alta, rezervarile care se suprapun, incercarea de a anula o rezervare deja anulata si rezervarile care depasesc limita de patru ore.

## 6. If you had 30 more minutes
What would you improve and what risk did you leave in the project?

Terminan tot proiectul de facut.
